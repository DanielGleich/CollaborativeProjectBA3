using FishNet.Connection;
using FishNet.Managing;
using Steamworks;
using System;
using UnityEngine;

public class LobbyConnectionManager : Singleton<LobbyConnectionManager>
{
    [SerializeField] private static int _maxPlayerCount = 4;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private FishySteamworks.FishySteamworks _fishySteamworks;

    protected Callback<LobbyCreated_t> _lobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> _lobbyJoinRequestedCallback;
    protected Callback<LobbyEnter_t> _lobbyEnteredCallback;
    protected Callback<LobbyChatUpdate_t> _lobbyChatUpdateCallback;
    protected Callback<LobbyDataUpdate_t> _lobbyDataUpdateCallback;

    protected override bool _dontDestroyOnLoad => true;
    private static ulong _currentLobbyID;
    private static ulong _lobbyCreatorId;
    public static ulong CurrentLobbyID => _currentLobbyID;

    public static event Action OnLobbyJoined;
    public static event Action OnLobbyExited;
    public static event Action OnLobbyJoinedViaFriendlist;
    public static event Action<CSteamID> OnClientJoinOrLeaves;
    //public static event Action OnLobbyOwnerLeft;


    private void OnEnable()
    {
        _lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(LobbyCreated);
        _lobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(LobbyJoinRequested);
        _lobbyEnteredCallback = Callback<LobbyEnter_t>.Create(LobbyEntered);
        _lobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create(ClientLeaveOrJoin);
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _maxPlayerCount);
    }

    internal static void JoinLobbyByID(CSteamID steamID)
    {
        Debug.Log($"Attempting to joing lobby with ID:({steamID})");

        if (SteamMatchmaking.RequestLobbyData(steamID))
        {
            SteamMatchmaking.JoinLobby(steamID);
        } 
        else
        {
            Debug.LogWarning($"Failed to join lobby with ID:({steamID})");
        }
    }

    public static void LeaveLobby()
    {
        if (_currentLobbyID == 0 || new CSteamID(_currentLobbyID).m_SteamID == 0)
            return;

        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        _currentLobbyID = 0;

        s_instance._fishySteamworks.StopConnection(false);
        if (s_instance._networkManager.IsServerStarted)
        {
            s_instance._fishySteamworks.StopConnection(true);
        }
        OnLobbyExited?.Invoke();
    }

    public static NetworkConnection GetServerConnection()
    {
        if (s_instance._networkManager.ServerManager == null ||
            s_instance._networkManager.ServerManager.Clients.Count == 0)
            return null;

        if (s_instance._networkManager.ServerManager.Clients.TryGetValue(1, out NetworkConnection connection))
        {
            return connection;
        }

        foreach (var conn in s_instance._networkManager.ServerManager.Clients.Values)
        {
            if (conn != null && conn.IsActive)
                return conn;
        }

        return null;
    }

    private void LobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        _currentLobbyID = callback.m_ulSteamIDLobby;

        CSteamID lobbyID = new CSteamID(CurrentLobbyID);

        string _hostAddress = SteamUser.GetSteamID().ToString();
        SteamMatchmaking.SetLobbyData(lobbyID, "HostAddress", _hostAddress);
        SteamMatchmaking.SetLobbyData(lobbyID, "LobbyName", SteamFriends.GetPersonaName().ToString() + "'s Lobby");

        //set the client address and then start a connection as server
        _fishySteamworks.SetClientAddress(_hostAddress);
        _fishySteamworks.StartConnection(true);
        _lobbyCreatorId = SteamMatchmaking.GetLobbyOwner(lobbyID).m_SteamID;
        Debug.Log("Lobby Creation Successful");
    }

    private void LobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        _lobbyCreatorId = SteamMatchmaking.GetLobbyOwner(new CSteamID(_currentLobbyID)).m_SteamID;
        OnLobbyJoinedViaFriendlist?.Invoke();
    }

    private void LobbyEntered(LobbyEnter_t callback)
    {
        _currentLobbyID = callback.m_ulSteamIDLobby;
        //set the client address based on the lobby data and then start a connection as a client
        _fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress"));
        _fishySteamworks.StartConnection(false);
        OnLobbyJoined?.Invoke();
    }

    private void ClientLeaveOrJoin(LobbyChatUpdate_t callback)
    {
        //if (SteamMatchmaking.GetLobbyOwner(new CSteamID(_currentLobbyID)).m_SteamID != _lobbyCreatorId)
        //{
        //    OnLobbyOwnerLeft?.Invoke();
        //}

        OnClientJoinOrLeaves?.Invoke(new CSteamID(callback.m_ulSteamIDUserChanged));
    }

    public static int GetMaxPlayerCount()
    { 
        return _maxPlayerCount;
    }
}
