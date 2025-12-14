using System;
using System.Collections.Generic;
using System.Diagnostics;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;

public class TeamManager : NetworkSingleton<TeamManager>
{
    protected override bool _perClient { get; } = false;
    public static List<UITeamCard> allTeamCards = new List<UITeamCard>();

    public readonly SyncDictionary<int, Team> allTeams = new SyncDictionary<int, Team>();
    public readonly SyncHashSet<ulong> allPlayers = new SyncHashSet<ulong>();
    public static event Action OnTeamUpdate;

    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitTeams();
    }
    
    public bool IsTeamSlotAvailable(int teamId, TeamRole role, CSteamID playerId)
    {
        if (allTeams.ContainsKey(teamId))
        {
            switch (role)
            {
                case TeamRole.SCIENTIST:
                    return allTeams[teamId].scientistPlayer == CSteamID.Nil;

                case TeamRole.RAT:
                    return allTeams[teamId].ratPlayer == CSteamID.Nil;
            }
        }

        return false;
    }

    public bool IsPlayerOwningSlot(CSteamID playerId)
    {
        return allPlayers.Contains(playerId.m_SteamID);
    }

    public void RemovePlayerFromAllTeams(CSteamID playerId)
    {
        if (IsPlayerOwningSlot(playerId))
        {
            foreach (KeyValuePair<int, Team> team in allTeams)
            {
                if (team.Value.scientistPlayer == playerId)
                {
                    team.Value.scientistPlayer = CSteamID.Nil;
                }

                if (team.Value.ratPlayer == playerId)
                {
                    team.Value.ratPlayer = CSteamID.Nil;
                }
            }

            allPlayers.Remove(playerId.m_SteamID);
            TeamManager.OnTeamUpdate?.Invoke();
        }
    }

    public void AssignPlayerToTeamSlot(int teamId, TeamRole role, CSteamID playerId)
    {
        if (!allTeams.ContainsKey(teamId)) return;

        RemovePlayerFromAllTeams(playerId);

        switch (role)
        {
            case TeamRole.SCIENTIST:
                allTeams[teamId].scientistPlayer = playerId;
                break;
            case TeamRole.RAT:
                allTeams[teamId].ratPlayer = playerId;
                break;
        }

        if (!allPlayers.Contains(playerId.m_SteamID))
            allPlayers.Add(playerId.m_SteamID);

        OnTeamUpdate?.Invoke();
    }


    public void InitTeams()
    {
        foreach (UITeamCard t in allTeamCards)
        {
            t.OnRequestProfile += RequestSlot;
            allTeams.Add(t.currentTeam.id, t.currentTeam);
        }
    }

    private void RequestSlot(int teamId, TeamRole slot, ulong steamId)
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
        if (IsTeamSlotAvailable(teamId, slot, playerId))
        {
            AssignPlayerToTeamSlot(teamId, slot, playerId);  
            UpdateTeamSlotObservers(teamId, slot, steamId);  
        }
    }

    [ObserversRpc]
    private void UpdateTeamSlotObservers(int teamId, TeamRole slot, ulong steamId)
    {
        MainMenuManager.Instance.UpdateLobbyProfiles();
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
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }
}

