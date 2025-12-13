using System;
using System.Collections.Generic;
using FishNet.Object;
using Steamworks;
using UnityEngine;

[System.Serializable]
public enum TeamRole 
{ 
    SCIENTIST = 0,
    RAT = 1
}

public struct Team
{
    public int id;
    public CSteamID scientistPlayer;
    public CSteamID ratPlayer;
    public GameObject teamCard;
}

public class TeamMakingManager : NetworkSingleton<TeamMakingManager>
{
    protected override bool _perClient { get; } = false;
    public static Dictionary<UITeamCard, Team> allTeams = new Dictionary<UITeamCard, Team>();

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Start Client");
        InitTeams();
    }

    public void InitTeams() 
    {
        foreach (KeyValuePair<UITeamCard, Team> t in allTeams)
        {
            t.Key.OnRequestProfile += RequestSlot;
        }
    }

    private void RequestSlot(int teamId, TeamRole slot, ulong steamId)
    {
        Debug.Log("A");
        if (!IsClientInitialized)
        {
            return;
        }
        Debug.Log("B");

        RequestTeamSlotServerRPC(teamId, slot, steamId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeamSlotServerRPC(int teamId, TeamRole slot, ulong steamId)
    {
        Debug.Log("C");
        UpdateTeamSlot(teamId, slot, steamId);
    }

    [ObserversRpc]
    private void UpdateTeamSlot(int teamId, TeamRole slot, ulong steamId)
    {
        Debug.Log("D");
        foreach (KeyValuePair<UITeamCard, Team> t in allTeams)
        {
            if (t.Value.id == teamId && t.Key.IsSlotAvailable(slot))
            {
                t.Key.UpdateTeamSlotProfile(slot, new CSteamID(steamId));
            }
        }
    }
}

