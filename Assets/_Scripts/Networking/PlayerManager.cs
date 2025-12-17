using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    protected override bool _perClient => false;

    [SerializeField] private NetworkObject playerPrefab;
    int i = 1;

    public readonly SyncDictionary<CSteamID, NetworkConnection> AllPlayerConnections = new SyncDictionary<CSteamID, NetworkConnection>();
    public readonly SyncDictionary<CSteamID, NetworkObject> AllPlayerObjects = new SyncDictionary<CSteamID, NetworkObject>();
    
    public static UnityEvent<CSteamID> OnPlayerDisconnected = new UnityEvent<CSteamID>();
    public static UnityEvent<CSteamID> OnPlayerConnected = new UnityEvent<CSteamID>();

    private void Start()
    {
        if (!IsServerInitialized) { return; }
        InstanceFinder.ClientManager.OnRemoteConnectionState += OnRemoteConnectionStateChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void OnRemoteConnectionStateChanged(RemoteConnectionStateArgs args)
    {
        NetworkConnection c = InstanceFinder.ClientManager.Clients[args.ConnectionId];
        if (c == null) return;

        CSteamID playerId = new CSteamID(ulong.Parse(c.GetAddress()));

        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            AllPlayerConnections.Add(playerId, c);
        }

        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            if (AllPlayerObjects.ContainsKey(playerId))
            {
                int clientId = c.ClientId;
                AllPlayerObjects.Remove(playerId);
                OnPlayerDisconnected?.Invoke(playerId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectToServerRPC(NetworkConnection c = null)
    {
        CSteamID playerId = new CSteamID(ulong.Parse(c.GetAddress()));
        NetworkObject player = SpawnPlayer(playerId);
        AllPlayerObjects.Add(playerId, player);
        Spawn(player, c);
        PlayerConnectedClientRpc(playerId);
    }


    public NetworkObject SpawnPlayer(CSteamID playerId)
    {
        NetworkObject p = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        AssignPlayerToTeam(p, playerId);

        p.gameObject.name = "Player " + i.ToString();
        i++;
        return p;
    }

    [Server]
    void AssignPlayerToTeam(NetworkObject playerObject, CSteamID playerId)
    {
        if (!playerObject.TryGetComponent(out PlayerAssignment playerAssignment) || playerId == CSteamID.Nil)
        {
            Debug.LogWarning($"Team assignment did not work for {playerObject.name} | SteamId: {playerId.m_SteamID}");
            return;
        }

        foreach (var kvp in TeamManager.Instance.allTeams)
        {
            Team t = kvp.Value;
            if (t.scientistPlayer == playerId)
            {
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.SCIENTIST;
            }
            else if (t.ratPlayer == playerId)
            {
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.RAT;
            }
        }

        Transform spawnPoint = PlayerSpawnPointManager.Instance.GetSpawnPointForPlayer(playerAssignment.CurrentTeam.Value.id, playerAssignment.CurrentRole.Value);
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        playerObject.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
    }


    [ObserversRpc]
    public void PlayerConnectedClientRpc(CSteamID playerId)
    {
        OnPlayerConnected?.Invoke(playerId);
    }
}
