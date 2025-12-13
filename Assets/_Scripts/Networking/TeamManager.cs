using System;
using System.Collections.Generic;
using FishNet.Object;
using Steamworks;
using UnityEngine;

public class TeamManager : NetworkSingleton<TeamManager>
{
    protected override bool _perClient { get; } = false;
    public static List<UITeamCard> allTeams = new List<UITeamCard>();

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitTeams();
    }

    public void InitTeams() 
    {
        foreach (UITeamCard t in allTeams)
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
        if (Team.IsSlotAvailable(teamId, slot, playerId))
        {
            Team.AssignPlayerToSlot(teamId, slot, playerId);
            UpdateTeamSlot(teamId, slot, steamId);
        }
    }

    [ObserversRpc]
    private void UpdateTeamSlot(int teamId, TeamRole slot, ulong steamId)
    {
        Team.AssignPlayerToSlot(teamId, slot, new CSteamID(steamId));
        MainMenuManager.Instance.UpdateLobbyProfiles();
    }
}

