using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPointManager : NetworkSingleton<PlayerSpawnPointManager>
{
    protected override bool _perClient { get; } = false;

    [SerializeField] protected LayerMask hitLayer;
    protected List<BoxCollider> spawnBoxes = new List<BoxCollider>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkSceneManager.OnNetworkedSceneChanged += LoadSpawnPoints;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        NetworkSceneManager.OnNetworkedSceneChanged -= LoadSpawnPoints;
    }

    [Server]
    public void LoadSpawnPoints(string newScene)
    {
        if (newScene != "Game") return;

        Debug.Log($"[SERVER] LoadSpawnPoints {newScene} | boxes after: {spawnBoxes.Count}");

        spawnBoxes.Clear();
        var spawnPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.TryGetComponent(out BoxCollider col))
                spawnBoxes.Add(col);
        }
    }

    [Server]
    public Transform GetSpawnPointForPlayer(int teamId, TeamRole role)
    {
        Debug.LogError($"GetSpawnPointForPlayer on {(IsServer ? "SERVER" : "CLIENT")} | boxes: {spawnBoxes.Count}");
        foreach (BoxCollider spawn in spawnBoxes)
        {
            if (spawn.TryGetComponent<PlayerSpawnPoint>(out PlayerSpawnPoint playerSpawn))
            {
                if (playerSpawn.teamId == teamId && playerSpawn.spawnPointType == role)
                {
                    return spawn.transform;
                }
            }
        }
        Debug.LogWarning($"No spawn found for team {teamId} role {role}");
        return null;
    }
}
