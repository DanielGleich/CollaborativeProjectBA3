using FishNet.Connection;

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
    public int scientistPlayerClientId;
    public int ratPlayerClientId;
}
