using System;
using System.Collections.Generic;
using System.Diagnostics;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using UnityEngine;

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
        allTeamCards = new List<UITeamCard>(FindObjectsByType<UITeamCard>(FindObjectsSortMode.None));

        foreach (UITeamCard t in allTeamCards)
        {
            t.OnRequestProfile += RequestSlot;
        }

        allTeams.OnChange += AllTeams_OnChange;
        allPlayers.OnChange += AllPlayers_OnChange;
    }

    private void AllPlayers_OnChange(SyncHashSetOperation op, ulong item, bool asServer)
    {
        MainMenuManager.Instance.UpdateLobbyProfiles();
        OnTeamUpdate?.Invoke();
    }

    private void AllTeams_OnChange(SyncDictionaryOperation op, int key, Team value, bool asServer)
    {
        MainMenuManager.Instance.UpdateLobbyProfiles();
        OnTeamUpdate?.Invoke();
    }

    public bool IsTeamSlotAvailable(int teamId, TeamRole role, CSteamID playerId)
    {
        if (!IsServerInitialized || !allTeams.ContainsKey(teamId)) return false;
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
        return IsServerInitialized && allPlayers.Contains(playerId.m_SteamID);
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
        UnityEngine.Debug.Log("Request Slot E");
        if (!allTeams.ContainsKey(teamId)) return;

        UnityEngine.Debug.Log("Request Slot F");
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
        if (!IsServerInitialized) return;

        allTeamCards = new List<UITeamCard>(FindObjectsByType<UITeamCard>(FindObjectsSortMode.None));
        allTeams.Clear();
        foreach (UITeamCard t in allTeamCards)
        {
            if (!allTeams.ContainsKey(t.currentTeam.id))
                allTeams.Add(t.currentTeam.id, t.currentTeam);
        }
    }

    private void RequestSlot(int teamId, TeamRole slot, ulong steamId)
    {
        UnityEngine.Debug.Log("Request Slot A");
        if (!IsClientInitialized)
        {
            return;
        }
        UnityEngine.Debug.Log("Request Slot B");
        RequestTeamSlotServerRPC(teamId, slot, steamId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeamSlotServerRPC(int teamId, TeamRole slot, ulong steamId)
    {
        UnityEngine.Debug.Log("Request Slot C");
        CSteamID playerId = new CSteamID(steamId);
        if (IsTeamSlotAvailable(teamId, slot, playerId))
        {
            UnityEngine.Debug.Log("Request Slot D");
            AssignPlayerToTeamSlot(teamId, slot, playerId);  
            UpdateTeamSlotObservers(teamId, slot, steamId);  
        }
    }

    [ObserversRpc]
    private void UpdateTeamSlotObservers(int teamId, TeamRole slot, ulong steamId)
    {
        UnityEngine.Debug.Log("Request Slot G");
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestLeaveTeamServerRPC(ulong steamId)
    {
        UnityEngine.Debug.Log("Request Team A");
        RemovePlayerFromAllTeams(new CSteamID(steamId));
        NotifyPlayerLeftObservers(steamId);              
    }

    [ObserversRpc]
    private void NotifyPlayerLeftObservers(ulong steamId)
    {
        UnityEngine.Debug.Log("Request Team B");
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }
}

