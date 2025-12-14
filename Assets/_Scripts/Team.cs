using UnityEngine;
using Steamworks;

public enum TeamRole
{
    SCIENTIST = 0,
    RAT = 1
}

[System.Serializable]
public class Team
{    
    public int id;
    public CSteamID scientistPlayer;
    public CSteamID ratPlayer;
    public GameObject teamCard;
}
