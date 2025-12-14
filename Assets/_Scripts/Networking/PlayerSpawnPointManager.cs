using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPointManager : NetworkSingleton<PlayerSpawnPointManager>
{
    protected override bool _perClient { get; } = false;

    [SerializeField] protected Transform spawnPointParent;
    [SerializeField] protected LayerMask hitLayer;
    protected List<BoxCollider> spawnBoxes = new List<BoxCollider>();
    void Awake()
    {
        foreach (Transform t in spawnPointParent)
        {
            BoxCollider collider = t.GetComponent<BoxCollider>();
            if (collider != null)
            {
                spawnBoxes.Add(collider);
            }
            else
            {
                Debug.Log($"{t.gameObject} has no BoxCollider");
            }
        }
    }

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
