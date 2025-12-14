using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : NetworkBehaviour
{

    [SerializeField] protected Transform spawnPointParent;
    [SerializeField] protected LayerMask hitLayer;
    protected List<BoxCollider> spawnBoxes = new List<BoxCollider>();
    void Awake()
    {
        Init();
    }

    protected void Init()
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

    public virtual Transform GetFreeRandomSpawnPoint()
    {
        List<Transform> freeSpawnpoints = new List<Transform>();
        foreach (BoxCollider c in spawnBoxes)
        {
            Vector3 worldCenter = c.transform.position + c.center;
            Collider[] hits = Physics.OverlapBox(worldCenter, c.size / 2, c.transform.rotation, hitLayer);
            if (hits.Length == 0)
            {
                freeSpawnpoints.Add(c.transform);
            }
        }

        if (freeSpawnpoints.Count > 0)
        {
            return freeSpawnpoints[UnityEngine.Random.Range(0, freeSpawnpoints.Count)];
        }
        else
        {
            Debug.Log($"No Spawnpoint found for {gameObject.name}");
            return spawnPointParent.GetChild(0);
        } 
    }
}
