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
    public static List<Team> allTeams = new List<Team>();
    public int id;
    public CSteamID scientistPlayer;
    public CSteamID ratPlayer;
    public GameObject teamCard;

    public static event Action OnTeamUpdate;

    public Team()
    {
        id = allTeams.Count;
        scientistPlayer = CSteamID.Nil;
        ratPlayer = CSteamID.Nil;
        allTeams.Add(this);
    }

    ~Team()
    {
        allTeams.Remove(this);
    }

    public static bool IsSlotAvailable(int teamId, TeamRole role, CSteamID playerId)
    {
        foreach (Team team in allTeams)
        {
            if (teamId == team.id)
            {
                if (role == TeamRole.SCIENTIST && team.scientistPlayer == CSteamID.Nil)
                {
                    return true;
                }

                if (role == TeamRole.RAT && team.ratPlayer == CSteamID.Nil)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsPlayerOwningSlot(CSteamID playerId)
    {
        foreach (Team t in allTeams)
        {
            if (t.scientistPlayer == playerId || t.ratPlayer == playerId)
            {
                return true;
            }
        }
        return false;
    }

    public static void AssignPlayerToSlot(int teamId, TeamRole role, CSteamID playerId)
    {
        foreach (Team team in allTeams)
        {
            if (team.scientistPlayer == playerId)
            {
                team.scientistPlayer = CSteamID.Nil;
                OnTeamUpdate?.Invoke();
            }

            if (team.ratPlayer == playerId)
            {
                team.ratPlayer = CSteamID.Nil;
                OnTeamUpdate?.Invoke();
            }

            if (team.id == teamId)
            {
                switch (role)
                {
                    case TeamRole.SCIENTIST:
                        team.scientistPlayer = playerId;
                        OnTeamUpdate?.Invoke();
                        break;

                    case TeamRole.RAT:
                        team.ratPlayer = playerId;
                        OnTeamUpdate?.Invoke();
                        break;
                }
            }
        }
    }
}
