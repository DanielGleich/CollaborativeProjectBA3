using UnityEngine;
using Steamworks;
using System;
using System.Collections.Generic;

public enum TeamRole
{
    SCIENTIST = 0,
    RAT = 1
}

public class Team
{
    public static Dictionary<int, Team> allTeams = new Dictionary<int, Team>();
    public static HashSet<CSteamID> allPlayers = new HashSet<CSteamID>();
    public int id;
    public CSteamID scientistPlayer;
    public CSteamID ratPlayer;
    public GameObject teamCard;

    public static event Action OnTeamUpdate;

    public Team(int id)
    {
        this.id = id;
        scientistPlayer = CSteamID.Nil;
        ratPlayer = CSteamID.Nil;
        allTeams.Add(id, this);
    }

    public static bool IsSlotAvailable(int teamId, TeamRole role, CSteamID playerId)
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

    public static bool IsPlayerOwningSlot(CSteamID playerId)
    {
        return allPlayers.Contains(playerId);
    }

    public static void RemovePlayer(CSteamID playerId)
    {
        if (IsPlayerOwningSlot(playerId))
        {
            foreach (var team in allTeams)
            {
                if (team.Value.scientistPlayer == playerId)
                    team.Value.scientistPlayer = CSteamID.Nil;

                if (team.Value.ratPlayer == playerId)
                    team.Value.ratPlayer = CSteamID.Nil;
            }

            allPlayers.Remove(playerId);
        }
    }

    public static void AssignPlayerToSlot(int teamId, TeamRole role, CSteamID playerId)
    {
        if (!allTeams.ContainsKey(teamId)) return;

        RemovePlayer(playerId);

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
}
