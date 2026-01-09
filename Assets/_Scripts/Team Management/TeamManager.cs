using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using System;
using System.Collections;
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
    public readonly SyncDictionary<int, TeamReadyFlag> isTeamReady = new SyncDictionary<int, TeamReadyFlag>();

    public static event Action OnTeamManagerCreated;
    public static event Action OnTeamUpdate;

    public static event Action<Team> OnTeamReady;
    public static event Action OnAllTeamsReady;

    public readonly SyncVar<bool> AllTeamsReady = new SyncVar<bool>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnTeamManagerCreated?.Invoke();
        allTeams.OnChange += TeamUpdate;
        isTeamReady.OnChange += OnPlayerReady;
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

    private void TeamUpdate(SyncDictionaryOperation op, int key, Team value, bool asServer)
    {
        OnTeamUpdate?.Invoke();
    }

    [Server]
    private void ClientJoinOrLeave(CSteamID _)
    {
        HashSet<NetworkConnection> connections = new();
        foreach (var kvp in ServerManager.Clients)
        { 
            connections.Add(kvp.Value);
        }

        foreach (var kvp in allTeams)
        {
            var team = kvp.Value;
            RemoveIfMissing(connections, team.scientistPlayer);
            RemoveIfMissing(connections, team.ratPlayer);
        }
    }

    private void RemoveIfMissing(HashSet<NetworkConnection> activeIds, NetworkConnection playerConnection)
    {
        if (playerConnection != null && !activeIds.Contains(playerConnection))
            RemovePlayerFromAllTeams(playerConnection);
    }

    public bool IsPlayerOwningSlot(NetworkConnection playerConnection)
    {
        foreach (var kvp in allTeams)
        {
            var t = kvp.Value;
            if (t.scientistPlayer == playerConnection || t.ratPlayer == playerConnection)
                return true;
        }
        return false;
    }

    public void RemovePlayerFromAllTeams(NetworkConnection playerConnection)
    {
        foreach (int teamId in allTeams.Keys.ToArray())
        {
            var team = allTeams[teamId];

            if (team.scientistPlayer == playerConnection)
                team.scientistPlayer = null;

            if (team.ratPlayer == playerConnection)
                team.ratPlayer = null;

            allTeams[teamId] = team;
        }
    }

    public void AssignPlayerToTeamSlot(int teamId, TeamRole role, NetworkConnection playerConnection)
    {
        if (!allTeams.TryGetValue(teamId, out Team team)) return;

        switch (role)
        {
            case TeamRole.SCIENTIST: team.scientistPlayer = playerConnection; break;
            case TeamRole.RAT: team.ratPlayer = playerConnection; break;
        }

        allTeams[teamId] = team;
    }

    public void InitTeams()
    {
        if (!IsServerInitialized) return;
        allTeams.Clear();

        for (int i = 0; i < maxTeamCount; i++)
        {
            Team newTeam = new Team() { id = i };
            allTeams.Add(i, newTeam);
            isTeamReady.Add(i, new TeamReadyFlag() { ratReady = false, scientistReady = false });
        }
    }

    public void RequestSlot(int teamId, TeamRole slot, NetworkConnection playerConnection)
    {
        if (!IsClientInitialized)
        {
            return;
        }
        RequestTeamSlotServerRPC(teamId, slot, playerConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeamSlotServerRPC(int teamId, TeamRole slot, NetworkConnection playerConnection)
    {
        RemovePlayerFromAllTeams(playerConnection);


        if (!allTeams.ContainsKey(teamId) ||
            (slot == TeamRole.SCIENTIST && allTeams[teamId].scientistPlayer != null) ||
            (slot == TeamRole.RAT && allTeams[teamId].ratPlayer != null))
        {
            return;
        }
        AssignPlayerToTeamSlot(teamId, slot, playerConnection);
        foreach (Team t in allTeams.Values)
        {
            Debug.Log($"Team {t.id} - S: {(t.scientistPlayer == null ? 0 : t.scientistPlayer.ClientId)} & R: {(t.ratPlayer == null ? 0 : t.ratPlayer.ClientId)}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestLeaveTeamServerRPC(NetworkConnection playerConnection)
    {
        RemovePlayerFromAllTeams(playerConnection);
        NotifyPlayerLeftObservers(playerConnection);
    }

    [ObserversRpc]
    private void NotifyPlayerLeftObservers(NetworkConnection playerConnection)
    {
        MainMenuManager.Instance?.UpdateLobbyProfiles();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReady(Team team, TeamRole teamRole)
    {
        if (isTeamReady.TryGetValue(team.id, out TeamReadyFlag teamReady))
        {
            if (teamRole == TeamRole.SCIENTIST)
            {
                teamReady.scientistReady = true;
            }
            else if (teamRole == TeamRole.RAT)
            {
                teamReady.ratReady = true;
            }
            isTeamReady[team.id] = teamReady;
        }
    }

    private void OnPlayerReady(SyncDictionaryOperation op, int key, TeamReadyFlag value, bool asServer)
    {
        if (value.scientistReady && value.ratReady)
        {
            if (asServer)
            {
                StartCoroutine(DelayedTeamReady(allTeams[key]));
            }
            else
            {
                OnTeamReady?.Invoke(allTeams[key]);
            }
        }
    }

    [Server]
    private IEnumerator DelayedTeamReady(Team team)
    {
        while (PlayerManager.Instance.GetNetworkObjectBySteamID(team.scientistPlayer) == null ||
               PlayerManager.Instance.GetNetworkObjectBySteamID(team.ratPlayer) == null)
        {
            yield return null;
        }

        OnTeamReady?.Invoke(team); 
        CheckAllTeamsReady();
    }

    private void CheckAllTeamsReady()
    {
        int i = 0;
        foreach (var kvp in isTeamReady)
        {
            if (kvp.Value.scientistReady && kvp.Value.ratReady)
                i++;
            else
                break;
        }

        if (isTeamReady.Count == i)
        {
            AllTeamsReady.Value = true;
            NotifyAllTeamsReady();
        }
    }

    [ObserversRpc]
    private void NotifyAllTeamsReady()
    {
        OnAllTeamsReady?.Invoke();
    }

    public bool IsTeamReady(Team team)
    {
        return IsTeamReady(team.id);
    }

    public bool IsTeamReady(int teamId)
    {
        if (isTeamReady.TryGetValue(teamId, out TeamReadyFlag teamReadyFlag))
            return (teamReadyFlag.scientistReady && teamReadyFlag.ratReady);
        Debug.LogError(IsServerInitialized ? "[Server]" : "[Client]" + $"Team with id {teamId} not found!");
        return false;
    }
    public NetworkObject GetOtherTeamMember(NetworkConnection playerConnection)
    {
        foreach (var team in allTeams)
        {
            if (team.Value.scientistPlayer == playerConnection)
                return PlayerManager.Instance.GetNetworkObjectBySteamID(team.Value.ratPlayer);
            else if (team.Value.ratPlayer == playerConnection)
                return PlayerManager.Instance.GetNetworkObjectBySteamID(team.Value.scientistPlayer);
        }
        return null;
    }

    public NetworkObject GetTeamMember(int teamId, TeamRole role)
    {
        if (allTeams.TryGetValue(teamId, out Team t))
        {
            return PlayerManager.Instance.GetNetworkObjectBySteamID(role == TeamRole.SCIENTIST ? t.scientistPlayer : t.ratPlayer);
        }
        return null;
    }
}

