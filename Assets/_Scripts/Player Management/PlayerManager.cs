using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Steamworks;
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

    private int dummyOffset = 100;

    private void Start()
    {
        if (!IsServerInitialized) { return; }
        InstanceFinder.ClientManager.OnRemoteConnectionState += OnRemoteConnectionStateChanged;
    }

    private void OnRemoteConnectionStateChanged(RemoteConnectionStateArgs args)
    {
        NetworkConnection c = InstanceFinder.ClientManager.Clients[args.ConnectionId];
        if (c == null) return;

        CSteamID playerId;

        if (c.GetAddress() == "127.0.0.1")
        {
            playerId = new CSteamID((ulong) (c.ClientId + dummyOffset));
        }
        else
        {
            playerId = new CSteamID(ulong.Parse(c.GetAddress()));
        }

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
        CSteamID playerId = CSteamID.Nil;
        bool isDummy = false;
        foreach (var kvp in AllPlayerConnections)
        {
            if (kvp.Value == c)
            { 
                playerId = kvp.Key;
                if (kvp.Key == new CSteamID((ulong)(c.ClientId + dummyOffset)))
                    isDummy = true;
                break;
            }
        }

        if (playerId == CSteamID.Nil) return;

        NetworkObject player = SpawnPlayer(playerId);

        if (isDummy)
            AutoAssignDummyToTeam(player, playerId);
        else 
            AssignPlayerToTeam(player, playerId);

        MovePlayerToSpawnPoint(player);
        AllPlayerObjects.Add(playerId, player);
        Spawn(player, c);
        PlayerConnectedClientRpc(playerId);
    }

    public NetworkObject SpawnPlayer(CSteamID playerId)
    {
        NetworkObject p = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        p.gameObject.name = "Player " + i.ToString();
        i++;
        return p;
    }

    [Server]
    void AssignPlayerToTeam(NetworkObject playerObject, CSteamID playerId)
    {
        if (!playerObject.TryGetComponent(out TeamMember playerAssignment) || playerId == CSteamID.Nil) return;

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
    }

    [Server]
    void AutoAssignDummyToTeam(NetworkObject playerObject, CSteamID playerId)
    {
        if (!playerObject.TryGetComponent(out TeamMember playerAssignment) || playerId == CSteamID.Nil) return;

        int lastTeamId = -1;
        bool assignedToTeam = false;

        foreach (var kvp in TeamManager.Instance.allTeams)
        {
            Team t = kvp.Value;
            lastTeamId = kvp.Value.id;

            if (t.ratPlayer == CSteamID.Nil)
            {
                TeamManager.Instance.AssignPlayerToTeamSlot(kvp.Value.id, TeamRole.RAT, playerId);
                playerAssignment.CurrentTeam.Value = t;
                playerAssignment.CurrentRole.Value = TeamRole.RAT;
                assignedToTeam = true;
                break;
            }
        }

        if (assignedToTeam == false)
        {
            int newId = lastTeamId + 1;
            Team newTeam = new Team() { id = newId, scientistPlayer = playerId };
            TeamManager.Instance.allTeams.Add(newId, newTeam);

            playerAssignment.CurrentTeam.Value = newTeam;
            playerAssignment.CurrentRole.Value = TeamRole.SCIENTIST;
        }
    }

    [Server]
    void MovePlayerToSpawnPoint(NetworkObject player)
    {
        if (player.TryGetComponent<TeamMember>(out TeamMember playerAssignment))
        {
            var team = playerAssignment.CurrentTeam.Value;
            var role = playerAssignment.CurrentRole.Value;
            Debug.Log($"{PlayerSpawnPointManager.Instance} - {IsServerInitialized}");
            Transform spawnPoint = PlayerSpawnPointManager.Instance.GetSpawnPointForPlayer(team.id, role);
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            player.transform.position = spawnPos;
        }
    }

    [ObserversRpc]
    public void PlayerConnectedClientRpc(CSteamID playerId)
    {
        OnPlayerConnected?.Invoke(playerId);
    }
}
