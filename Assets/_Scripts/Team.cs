using Steamworks;
using UnityEngine;

public enum TeamRole
{
    INVALID = -1,
    SCIENTIST = 0,
    RAT = 1
}

[System.Serializable]
public struct Team
{    
    public int id;
    public CSteamID scientistPlayer;
    public CSteamID ratPlayer;
}
