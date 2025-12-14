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
        InitTeams();
        LobbyConnectionManager.OnClientJoinOrLeaves += ClientJoinOrLeave;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        LobbyConnectionManager.OnClientJoinOrLeaves -= ClientJoinOrLeave;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SyncTeamTemplateServerRpc(int teamId, Team template)
    {
        if (!allTeams.ContainsKey(teamId))
            allTeams.Add(teamId, template); 
    }

    [Server]
    private void ClientJoinOrLeave(CSteamID obj)
    {
        List<ulong> activePlayers = new List<ulong>();
        CSteamID lobbyId = new CSteamID(LobbyConnectionManager.CurrentLobbyID);
        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyId); i++)
        {
            activePlayers.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i).m_SteamID);
        }

        List<CSteamID> playersToRemove = new List<CSteamID>();
        foreach (ulong playerId in allPlayers)
        {
            if (activePlayers.Contains(playerId) == false)
            {
                playersToRemove.Add(new CSteamID(playerId));
            }
        }

        foreach(CSteamID id in playersToRemove)
            RemovePlayerFromAllTeams(id);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        allTeamCards = new List<UITeamCard>(FindObjectsByType<UITeamCard>(FindObjectsSortMode.None));

        foreach (UITeamCard t in allTeamCards)
        {
            t.OnRequestProfile += RequestSlot;
            SyncTeamTemplateServerRpc(t.currentTeamId, t.teamTemplate);
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
        return allPlayers.Contains(playerId.m_SteamID);
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

        UnityEngine.Debug.Log("Request Slot D");
        if (!allTeams.TryGetValue(teamId, out Team team)) return;
        UnityEngine.Debug.Log("Request Slot E");

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
        UnityEngine.Debug.Log("Request Slot C");
        UnityEngine.Debug.Log(allTeams.Count());
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
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }
}

