using System;
using System.Collections.Generic;
using FishNet.Object;
using Steamworks;
using UnityEngine;

public class TeamManager : NetworkSingleton<TeamManager>
{
    protected override bool _perClient { get; } = false;
    public static List<UITeamCard> allTeamCards = new List<UITeamCard>();

    public static Dictionary<int, Team> allTeams = new Dictionary<int, Team>();
    public static HashSet<CSteamID> allPlayers = new HashSet<CSteamID>();
    public static event Action OnTeamUpdate;

    private void Awake()
    {
        ClearStaticLists();
    }

    public static void ClearStaticLists()
    {
        allTeams.Clear();
        allPlayers.Clear();
        foreach (UITeamCard card in allTeamCards)
        { 
            Destroy(card);
        }
        allTeamCards.Clear();
    }

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
        return allPlayers.Contains(playerId);
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

            allPlayers.Remove(playerId);
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

        if (!allPlayers.Contains(playerId))
            allPlayers.Add(playerId);

        OnTeamUpdate?.Invoke();
    }


    public void InitTeams()
    {
        foreach (UITeamCard t in allTeamCards)
        {
            t.OnRequestProfile += RequestSlot;
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
            UpdateTeamSlot(teamId, slot, steamId);
        }
    }

    [ObserversRpc]
    private void UpdateTeamSlot(int teamId, TeamRole slot, ulong steamId)
    {
        AssignPlayerToTeamSlot(teamId, slot, new CSteamID(steamId));
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestLeaveTeamServerRPC(ulong steamId)
    {
        RemovePlayerFromAllTeams(new CSteamID(steamId));
        RemovePlayerFromTeam(steamId);
    }

    [ObserversRpc]
    private void RemovePlayerFromTeam(ulong steamId)
    { 
        RemovePlayerFromAllTeams(new CSteamID(steamId));
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }
}

