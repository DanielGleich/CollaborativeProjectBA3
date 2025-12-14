using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
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
        InitTeamsServerRpc();

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
        if (!IsPlayerOwningSlot(playerId)) return;

        foreach (int teamId in allTeams.Keys.ToArray())  
        {
            if (allTeams.TryGetValue(teamId, out Team team))
            {
                Team temp = new Team() { id = team.id, ratPlayer = team.ratPlayer, scientistPlayer = team.scientistPlayer };

                if (team.scientistPlayer == playerId)
                {
                    temp.scientistPlayer = CSteamID.Nil;
                }

                if (team.ratPlayer == playerId)
                {
                    temp.ratPlayer = CSteamID.Nil;
                }

                allTeams[teamId] = temp;
            }
        }

        allPlayers.Remove(playerId.m_SteamID);
    }


    public void AssignPlayerToTeamSlot(int teamId, TeamRole role, CSteamID playerId)
    {
        if (!allTeams.TryGetValue(teamId, out Team team)) return;

        // RemovePlayerFromAllTeams bereits in RPC aufgerufen!

        switch (role)
        {
            case TeamRole.SCIENTIST: team.scientistPlayer = playerId; break;
            case TeamRole.RAT: team.ratPlayer = playerId; break;
        }

        allTeams[teamId] = team;

        if (!allPlayers.Contains(playerId.m_SteamID))
            allPlayers.Add(playerId.m_SteamID);

        OnTeamUpdate?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitTeamsServerRpc()
    {
        InitTeams();
    }

    public void InitTeams()
    {
        if (!IsServerInitialized) return;

        allTeamCards = new List<UITeamCard>(FindObjectsByType<UITeamCard>(FindObjectsSortMode.None));
        allTeams.Clear();
        foreach (UITeamCard t in allTeamCards)
        {
            if (!allTeams.ContainsKey(t.currentTeamId))
                allTeams.Add(t.currentTeamId, t.teamTemplate);
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
        Debug.Log("Request Slot C");
        CSteamID playerId = new CSteamID(steamId);

        // 1. ZURÜCKSETZEN (alte Slots leeren)
        RemovePlayerFromAllTeams(playerId);

        // 2. JETZT lokalen Slot prüfen (ist jetzt frei)
        if (!allTeams.ContainsKey(teamId) ||
            (slot == TeamRole.SCIENTIST && allTeams[teamId].scientistPlayer != CSteamID.Nil) ||
            (slot == TeamRole.RAT && allTeams[teamId].ratPlayer != CSteamID.Nil))
        {
            Debug.Log("Slot nicht verfügbar nach Reset");
            return;
        }

        // 3. NEU ZUWEISEN
        Debug.Log("Request Slot D");
        AssignPlayerToTeamSlot(teamId, slot, playerId);
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

