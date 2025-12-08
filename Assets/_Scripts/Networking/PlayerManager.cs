using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    protected override bool _perClient => false;

    private Dictionary<int, NetworkObject> _allPlayers = new Dictionary<int, NetworkObject>();
    public static NetworkObject LocalPlayer;
    public static NetworkObject[] AllPlayers { get { return s_instance.GetAllPlayers(); } }
    SpawnPointManager spawnPointManager;
    
    public static UnityEvent<NetworkObject> OnPlayerDisconnected = new UnityEvent<NetworkObject>();
    public static UnityEvent<NetworkObject> OnPlayerConnected = new UnityEvent<NetworkObject>();
    public static UnityEvent<NetworkObject> OnLocalPlayerConnected = new UnityEvent<NetworkObject>();



    private void Start()
    {
        if (!IsServerInitialized) { return; }
        spawnPointManager = GetComponent<SpawnPointManager>();
        InstanceFinder.ClientManager.OnRemoteConnectionState += OnRemoteConnectionStateChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ConnectToServerRPC();
    }

    private void OnRemoteConnectionStateChanged(RemoteConnectionStateArgs args)
    {
        NetworkConnection c = InstanceFinder.ClientManager.Clients[args.ConnectionId];
        if (c != null && args.ConnectionState == RemoteConnectionState.Stopped)
        {
            if (_allPlayers.ContainsKey(c.ClientId))
            { 
                OnPlayerDisconnected?.Invoke(_allPlayers[c.ClientId]);
                _allPlayers.Remove(c.ClientId);
            }
        }
    }

    public NetworkObject[] GetAllPlayers()
    {
        return _allPlayers.Values.ToArray();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectToServerRPC(NetworkConnection connection = null)
    {
        NetworkObject player = PlayerSpawnController.SpawnPlayer(spawnPointManager.GetFreeRandomSpawnPoint().position);
        _allPlayers.Add(connection.ClientId, player);
        Spawn(player, connection);
        PlayerConnectedClientRpc(player);
    }

    [ObserversRpc]
    public void PlayerConnectedClientRpc(NetworkObject playerObject)
    {
        OnPlayerConnected?.Invoke(playerObject);
        if (playerObject.IsOwner)
        {
            LocalPlayer = playerObject;
            OnLocalPlayerConnected?.Invoke(playerObject);
        }
    }
}
