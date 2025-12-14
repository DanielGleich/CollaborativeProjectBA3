using Steamworks;
using UnityEngine;

public enum TeamRole
{
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
