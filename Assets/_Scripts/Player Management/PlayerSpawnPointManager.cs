using FishNet.Object;
using System;
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
        LoadSpawnPoints();
    }

    [Server]
    public void LoadSpawnPoints()
    {
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
        return null;
    }
}
