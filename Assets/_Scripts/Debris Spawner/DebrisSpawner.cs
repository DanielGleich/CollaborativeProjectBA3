using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Debris[] debris;

    [Header("Settings")]
    [SerializeField] private int debrisCount = 3;
    [SerializeField] private float spawnRadius = 3;
    [SerializeField] private Vector3 spawnOffset = new(0,0,0);
    [SerializeField] private float spawnDelay = 0.1f;

    public event Action<bool> OnStartSpawn;
    public event Action OnSpawnSingle;


    public void SpawnDebris() => StartCoroutine(SpawnRoutine(debrisCount));
    public void SpawnDebris(int debrisCount) => StartCoroutine(SpawnRoutine(debrisCount));

    private IEnumerator SpawnRoutine(int debrisCount)
    {
        OnStartSpawn?.Invoke(true);
        WaitForSeconds waitDelay = new WaitForSeconds(spawnDelay);
        for(int i = 0; i < debrisCount; i++)
        {
            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-1,1) * spawnRadius, 0, UnityEngine.Random.Range(-1,1) * spawnRadius) + transform.position + spawnOffset;
            Instantiate(debris[UnityEngine.Random.Range(0,debris.Count())], spawnPos, UnityEngine.Random.rotation);
            OnSpawnSingle?.Invoke();
            yield return waitDelay;
        }
        OnStartSpawn?.Invoke(false);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}