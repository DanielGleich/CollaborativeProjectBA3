using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    protected override bool _perClient => false;

    [SerializeField] private NetworkObject playerPrefab;
    int i = 1;

    public readonly SyncList<NetworkConnection> AllPlayerConnections = new();
    
    public static UnityEvent<NetworkConnection> OnPlayerDisconnected = new();
    public static UnityEvent<NetworkConnection> OnPlayerConnected = new();

    private void Start()
    {
        if (!IsServerInitialized) { return; }
        InstanceFinder.ClientManager.OnRemoteConnectionState += OnRemoteConnectionStateChanged;
    }

    private void OnRemoteConnectionStateChanged(RemoteConnectionStateArgs args)
    {
        NetworkConnection c = InstanceFinder.ClientManager.Clients[args.ConnectionId];
        if (c == null) return;

        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            AllPlayerConnections.Add(c);
        }

        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            AllPlayerConnections.Remove(c);
            int clientId = c.ClientId;
            OnPlayerDisconnected?.Invoke(c);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectToServerRPC(NetworkConnection c = null)
    {
        if (c == null) return;

        NetworkObject player = SpawnPlayer();

        if (c.GetAddress() == "127.0.0.1")
            AutoAssignDummyToTeam(player, c);
        else 
            AssignPlayerToTeam(player, c);

        MovePlayerToSpawnPoint(player);
        Spawn(player, c);
        PlayerConnectedClientRpc(c);
    }

    public NetworkObject SpawnPlayer()
    {
        NetworkObject p = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        p.gameObject.name = "Player " + i.ToString();
        i++;
        return p;
    }

    [Server]
    void AssignPlayerToTeam(NetworkObject playerObject, NetworkConnection c)
    {
        if (!playerObject.TryGetComponent(out TeamMember playerAssignment)) return;

        foreach (var kvp in TeamManager.Instance.allTeams)
        {
            Team t = kvp.Value;
            if (t.scientistPlayer == c)
            {
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.SCIENTIST;
            }
            else if (t.ratPlayer == c)
            {
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.RAT;
            }
        }
    }

    [Server]
    void AutoAssignDummyToTeam(NetworkObject playerObject, NetworkConnection c)
    {
        if (!playerObject.TryGetComponent(out TeamMember playerAssignment)) return;

        foreach (var kvp in TeamManager.Instance.allTeams)
        {
            Team t = kvp.Value;

            if (t.scientistPlayer == null)
            { 
                TeamManager.Instance.AssignPlayerToTeamSlot(kvp.Value.id, TeamRole.SCIENTIST, c);
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.SCIENTIST;
                return;
            } 
            else if (t.ratPlayer == null)
            {
                TeamManager.Instance.AssignPlayerToTeamSlot(kvp.Value.id, TeamRole.RAT, c);
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.RAT;
                return;
            }
        }

        Debug.LogWarning($"No Team found for dummy {playerObject}");
    }

    [Server]
    void MovePlayerToSpawnPoint(NetworkObject player)
    {
        if (player.TryGetComponent<TeamMember>(out TeamMember playerAssignment))
        {
            var team = playerAssignment.CurrentTeam.Value;
            var role = playerAssignment.CurrentRole.Value;
            Transform spawnPoint = PlayerSpawnPointManager.Instance.GetSpawnPointForPlayer(team.id, role);
            player.transform.position = spawnPoint? spawnPoint.position : Vector3.zero;
            player.transform.rotation = spawnPoint? spawnPoint.rotation : Quaternion.identity;
        }
    }

    [ObserversRpc]
    public void PlayerConnectedClientRpc(NetworkConnection c)
    {
        OnPlayerConnected?.Invoke(c);
    }

    public NetworkObject GetNetworkObjectBySteamID(NetworkConnection c)
    {
        if (AllPlayerConnections.Contains(c))
        {
            if (base.IsServerInitialized && InstanceFinder.ServerManager.Clients.TryGetValue(c.ClientId, out NetworkConnection serverConn))
                return serverConn.FirstObject;

            return c.FirstObject;
        }
        return null;
    }
}
