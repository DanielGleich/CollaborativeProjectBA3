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

    public readonly SyncList<int> AllPlayerConnections = new SyncList<int>();
    public readonly SyncDictionary<int, ulong> AllPlayerSteamIds = new SyncDictionary<int, ulong>();
    
    public static UnityEvent<int> OnPlayerDisconnected = new UnityEvent<int>();
    public static UnityEvent<int> OnPlayerConnected = new UnityEvent<int>();

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
            AllPlayerConnections.Add(c.ClientId);
            if (ulong.TryParse(c.GetAddress(), out ulong steamId))
                AllPlayerSteamIds.Add(c.ClientId, steamId);
            NotifyPlayerConnected(c.ClientId);
        }

        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            AllPlayerConnections.Remove(c.ClientId);
            if (AllPlayerSteamIds.ContainsKey(c.ClientId))
            {
                AllPlayerSteamIds.Remove(c.ClientId);
            }
            NotifyPlayerDisconnected(c.ClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectToServerRPC(NetworkConnection c = null)
    {
        Debug.Log($"RPC Triggered- {c == null}");
        if (c == null) return;

        Debug.Log($"Player Spawn triggered - {c.GetAddress()}");

        NetworkObject player = SpawnPlayer();

        if (c.GetAddress() == "127.0.0.1")
            AutoAssignDummyToTeam(player, c.ClientId);
        else 
            AssignPlayerToTeam(player, c.ClientId);

        MovePlayerToSpawnPoint(player);
        Spawn(player, c);
    }

    public NetworkObject SpawnPlayer()
    {
        NetworkObject p = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        p.gameObject.name = "Player " + i.ToString();
        i++;
        return p;
    }

    [Server]
    void AssignPlayerToTeam(NetworkObject playerObject, int clientId)
    {
        if (!playerObject.TryGetComponent(out TeamMember playerAssignment)) return;

        foreach (var kvp in TeamManager.Instance.allTeams)
        {
            Team t = kvp.Value;
            if (t.scientistPlayerClientId == clientId)
            {
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.SCIENTIST;
            }
            else if (t.ratPlayerClientId == clientId)
            {
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.RAT;
            }
        }
    }

    [Server]
    void AutoAssignDummyToTeam(NetworkObject playerObject, int clientId)
    {
        if (!playerObject.TryGetComponent(out TeamMember playerAssignment)) return;

        foreach (var kvp in TeamManager.Instance.allTeams)
        {
            Team t = kvp.Value;

            if (t.scientistPlayerClientId == -1)
            { 
                TeamManager.Instance.AssignPlayerToTeamSlot(kvp.Value.id, TeamRole.SCIENTIST, clientId);
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.SCIENTIST;
                return;
            } 
            else if (t.ratPlayerClientId == -1)
            {
                TeamManager.Instance.AssignPlayerToTeamSlot(kvp.Value.id, TeamRole.RAT, clientId);
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
    public void NotifyPlayerConnected(int clientId)
    {
        OnPlayerConnected?.Invoke(clientId);
    }

    [ObserversRpc]
    public void NotifyPlayerDisconnected(int clientId)
    {
        OnPlayerDisconnected?.Invoke(clientId);
    }

    public NetworkObject GetNetworkObjectBySteamID(int clientId)
    {
        if (AllPlayerConnections.Contains(clientId))
        {
            if (base.IsServerInitialized && InstanceFinder.ServerManager.Clients.TryGetValue(clientId, out NetworkConnection serverConn))
                return serverConn.FirstObject;
        }
        return null;
    }
}
