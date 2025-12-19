using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TeamReadyFlag
{
    public bool ratReady;
    public bool scientistReady;
}

public class TeamManager : NetworkSingleton<TeamManager>
{
    protected override bool _perClient { get; } = false;
    
    [Header("Settings")]
    [SerializeField] int maxTeamCount = 2;

    public readonly SyncDictionary<int, Team> allTeams = new SyncDictionary<int, Team>();
    public readonly SyncDictionary<Team, TeamReadyFlag> isTeamReady = new SyncDictionary<Team, TeamReadyFlag>();

    public static event Action OnTeamManagerCreated;
    public static event Action OnTeamUpdate;

    public static event Action<Team> OnTeamReady;
    public static event Action OnAllTeamsReady;

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnTeamManagerCreated?.Invoke();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        InitTeams();
        LobbyConnectionManager.OnClientJoinOrLeaves += ClientJoinOrLeave;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        LobbyConnectionManager.OnClientJoinOrLeaves -= ClientJoinOrLeave;
    }


    [Server]
    private void ClientJoinOrLeave(CSteamID _)
    {
        var activeIds = GetActiveLobbyIds();

        foreach (var kvp in allTeams)
        {
            var team = kvp.Value;
            RemoveIfMissing(activeIds, team.scientistPlayer);
            RemoveIfMissing(activeIds, team.ratPlayer);
        }
    }

    private HashSet<ulong> GetActiveLobbyIds()
    {
        var ids = new HashSet<ulong>();
        var lobbyId = new CSteamID(LobbyConnectionManager.CurrentLobbyID);

        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyId); i++)
            ids.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i).m_SteamID);

        return ids;
    }

    private void RemoveIfMissing(HashSet<ulong> activeIds, CSteamID player)
    {
        if (player != CSteamID.Nil && !activeIds.Contains(player.m_SteamID))
            RemovePlayerFromAllTeams(player);
    }

    public bool IsPlayerOwningSlot(CSteamID playerId)
    {
        ulong id = playerId.m_SteamID;
        foreach (var kvp in allTeams)
        {
            var t = kvp.Value;
            if (t.scientistPlayer.m_SteamID == id || t.ratPlayer.m_SteamID == id)
                return true;
        }
        return false;
    }

    public void RemovePlayerFromAllTeams(CSteamID playerId)
    {
        ulong id = playerId.m_SteamID;

        foreach (int teamId in allTeams.Keys.ToArray())
        {
            var team = allTeams[teamId];

            if (team.scientistPlayer.m_SteamID == id)
                team.scientistPlayer = CSteamID.Nil;

            if (team.ratPlayer.m_SteamID == id)
                team.ratPlayer = CSteamID.Nil;

            allTeams[teamId] = team;
            OnTeamUpdate?.Invoke();
        }
    }

    public void AssignPlayerToTeamSlot(int teamId, TeamRole role, CSteamID playerId)
    {
        if (!allTeams.TryGetValue(teamId, out Team team)) return;

        switch (role)
        {
            case TeamRole.SCIENTIST: team.scientistPlayer = playerId; break;
            case TeamRole.RAT: team.ratPlayer = playerId; break;
        }

        allTeams[teamId] = team;

        OnTeamUpdate?.Invoke();
    }

    public void InitTeams()
    {
        if (!IsServerInitialized) return;
        allTeams.Clear();

        for (int i = 0; i < maxTeamCount; i++)
            allTeams.Add(i, new Team() { id = i });
    }

    public void RequestSlot(int teamId, TeamRole slot, ulong steamId)
    {
        if (!IsClientInitialized)
        {
            return;
        }
        RequestTeamSlotServerRPC(teamId, slot, steamId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeamSlotServerRPC(int teamId, TeamRole slot, ulong steamId)
    {
        CSteamID playerId = new CSteamID(steamId);

        RemovePlayerFromAllTeams(playerId);

        if (!allTeams.ContainsKey(teamId) ||
            (slot == TeamRole.SCIENTIST && allTeams[teamId].scientistPlayer != CSteamID.Nil) ||
            (slot == TeamRole.RAT && allTeams[teamId].ratPlayer != CSteamID.Nil))
        {
            return;
        }
        AssignPlayerToTeamSlot(teamId, slot, playerId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestLeaveTeamServerRPC(ulong steamId)
    {
        RemovePlayerFromAllTeams(new CSteamID(steamId));
        NotifyPlayerLeftObservers(steamId);
    }

    [ObserversRpc]
    private void NotifyPlayerLeftObservers(ulong steamId)
    {
        MainMenuManager.Instance?.UpdateLobbyProfiles();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReady(Team team, TeamRole teamRole)
    {
        if (isTeamReady.TryGetValue(team, out TeamReadyFlag teamReady))
        {
            if (teamRole == TeamRole.SCIENTIST)
            {
                teamReady.scientistReady = true;
            }
            else if (teamRole == TeamRole.RAT)
            {
                teamReady.ratReady = true;
            }
            if (teamReady.scientistReady && teamReady.ratReady)
                NotifyTeamReady(team);
        }
        else
        {
            TeamReadyFlag teamReadyFlag = new TeamReadyFlag();
            if (teamRole == TeamRole.SCIENTIST)
            {
                teamReadyFlag.scientistReady = true;
            }
            else if (teamRole == TeamRole.RAT)
            {
                teamReadyFlag.ratReady = true;
            }
            isTeamReady.Add(team, teamReadyFlag);
        }
    }

    [ObserversRpc]
    private void NotifyTeamReady(Team team)
    {
        Debug.Log($"Team {team.id} ready");
        OnTeamReady?.Invoke(team);
    }

    public bool IsTeamReady(Team team)
    {
        if (isTeamReady.TryGetValue(team, out TeamReadyFlag teamReadyFlag))
            return teamReadyFlag.scientistReady && teamReadyFlag.ratReady;

        Debug.LogError($"Team {team} not found!");
        return false;
    }

    public bool IsTeamReady(int teamId)
    {
        if (allTeams.TryGetValue(teamId, out Team team))
            return IsTeamReady(team);

        Debug.LogError($"Team with id {teamId} not found!");
        return false;
    }
}

