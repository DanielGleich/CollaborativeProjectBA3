using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPointManager : NetworkBehaviour
{

    [SerializeField] Transform _spawnPointParent;
    [SerializeField] LayerMask _hitLayer;
    List<BoxCollider> _spawnBoxes = new List<BoxCollider>();
    void Awake()
    {
        foreach (Transform t in _spawnPointParent)
        { 
            BoxCollider collider = t.GetComponent<BoxCollider>();
            if (collider != null)
            {
                _spawnBoxes.Add(collider);
            }
            else
            {
                Debug.Log($"{t.gameObject} has no BoxCollider");
            }
        }
    }

    public Transform GetFreeRandomSpawnPoint()
    {
        List<Transform> freeSpawnpoints = new List<Transform>();
        foreach (BoxCollider c in _spawnBoxes)
        {
            Vector3 worldCenter = c.transform.position + c.center;
            Collider[] hits = Physics.OverlapBox(worldCenter, c.size / 2, c.transform.rotation, _hitLayer);
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
            return _spawnPointParent.GetChild(0);
        } 
    }
}
