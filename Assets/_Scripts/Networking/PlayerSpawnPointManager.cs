using FishNet;
using FishNet.Managing.Scened;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPointManager : NetworkSingleton<PlayerSpawnPointManager>
{
    protected override bool _perClient { get; } = false;

    [SerializeField] protected Transform spawnPointParent;
    [SerializeField] protected LayerMask hitLayer;
    protected List<BoxCollider> spawnBoxes = new List<BoxCollider>();

    private void OnEnable()
    {
        NetworkSceneManager.OnNetworkedSceneChanged += LoadSpawnPoints;
    }

    private void OnDisable()
    {
        NetworkSceneManager.OnNetworkedSceneChanged -= LoadSpawnPoints;        
    }

    public void LoadSpawnPoints(string newScene)
    {
        if (newScene != "Game") return;

        PlayerSpawnPoint[] spawnPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        foreach (PlayerSpawnPoint spawnPoint in spawnPoints)
        {
            BoxCollider collider = spawnPoint.gameObject.GetComponent<BoxCollider>();
            Debug.Log("A");

            if (collider != null)
            {
                spawnBoxes.Add(collider);
            }
            else
            {
                Debug.Log($"{spawnPoint.gameObject} has no BoxCollider");
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
                    Debug.Log($"{playerSpawn.teamId} - {playerSpawn.spawnPointType}");
                    return spawn.transform;
                }
            }
        }
        return null;
    }
}
