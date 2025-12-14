using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using FishySteamworks;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    protected override bool _perClient => false;

    private Dictionary<int, NetworkObject> allPlayers = new Dictionary<int, NetworkObject>();
    public static NetworkObject[] AllPlayers { get { return s_instance.GetAllPlayers(); } }
    
    public static NetworkObject LocalPlayer;
    
    public static UnityEvent<NetworkObject> OnPlayerDisconnected = new UnityEvent<NetworkObject>();
    public static UnityEvent<NetworkObject> OnPlayerConnected = new UnityEvent<NetworkObject>();
    public static UnityEvent<NetworkObject> OnLocalPlayerConnected = new UnityEvent<NetworkObject>();

    private void Start()
    {
        if (!IsServerInitialized) { return; }
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
            if (allPlayers.ContainsKey(c.ClientId))
            { 
                OnPlayerDisconnected?.Invoke(allPlayers[c.ClientId]);
                allPlayers.Remove(c.ClientId);
            }
        }
    }

    public NetworkObject[] GetAllPlayers()
    {
        return allPlayers.Values.ToArray();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectToServerRPC(NetworkConnection connection = null)
    {
        NetworkObject player = PlayerSpawnController.SpawnPlayer(Vector3.zero);
        allPlayers.Add(connection.ClientId, player);
        Spawn(player, connection);
        PlayerConnectedClientRpc(player);
    }

    [Server]
    public void SetPlayerToSpawnPoint(NetworkObject player)
    {
        if (player.TryGetComponent<PlayerAssignment>(out PlayerAssignment playerAssignment))
        {
            Transform spawnPoint = PlayerSpawnPointManager.Instance.GetSpawnPointForPlayer(playerAssignment.CurrentTeam.id, playerAssignment.CurrentRole);
            player.transform.position = spawnPoint == null ? Vector3.zero : spawnPoint.position;
        }
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
