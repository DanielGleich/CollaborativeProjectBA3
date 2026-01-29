using FMODUnity;
using UnityEngine;

public class DebrisSpawnerFeedback : MonoBehaviour {
    [Header("References")]
    [SerializeField] private DebrisSpawner debrisSpawner;

    [Header("SFX")]
    [SerializeField] private EventReference debrisSpawnSFX;

    void OnValidate()
    {
        if(!debrisSpawner)
            debrisSpawner = GetComponentInChildren<DebrisSpawner>();
    }
    void OnEnable()
    {
        debrisSpawner.OnStartSpawn += StartSpawnFeedback;
        debrisSpawner.OnSpawnSingle += SpawnSingleFeedback;
    }
    void OnDisable()
    {
        debrisSpawner.OnStartSpawn -= StartSpawnFeedback;
        debrisSpawner.OnSpawnSingle -= SpawnSingleFeedback;
    }

    private void StartSpawnFeedback(bool obj)
    {
        
    }

    private void SpawnSingleFeedback()
    {
        RuntimeManager.PlayOneShot(debrisSpawnSFX, debrisSpawner.transform.position);
    }
}