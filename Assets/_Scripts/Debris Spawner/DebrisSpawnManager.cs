using System;
using System.Collections;
using System.Linq;
using GameKit.Dependencies.Utilities;
using UnityEngine;

public class DebrisSpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DebrisSpawner[] debrisSpawners;

    [Header("Settings")]
    [SerializeField] private bool startOnEnable = true;
    [SerializeField, Tooltip("time dependend functionalty starts after inital wait")] private float initialWait = 0f;

    [Header("Randomised Spawn Delay Settings")]
    [SerializeField, Tooltip("Only uses y-value of spawnDelayRange if active")] private bool constantSpawnDelay = false;
    [SerializeField, Min(0), Tooltip("x = Min, y = Max")] private Vector2 spawnDelayRange = new(10,20);

    [Header("Spawn Delay over Time Settings")]
    [SerializeField] private bool useSpawnDelayMultiplier = false;
    [SerializeField, Tooltip("Modifies the spawn frequency over time")] private AnimationCurve spawnDelayMultiplierCurve;

    [Header("Similtanous Spawns Settings"), Tooltip("COntrolls how many DebrisSpawner get triggered at the same time")]
    [SerializeField, Min(1)] private int similtanousSpawns = 1;
    [SerializeField] private bool useSimiltanousSpawnsMultiplier = false;
    [SerializeField] private AnimationCurve simultaniousSpawnsMultiplierCurve;

    [Header("Debris Count Settings")]
    [SerializeField, Tooltip("When set to false it will use the debris count of the DebrisSpawner")] private bool overwriteDebrisCount = true;
    [SerializeField, Min(0)] private Vector2Int debrisCountRange = new(1, 1);
    [SerializeField, Tooltip("Only uses y-vale of debrisPartsRange if active")] private bool constantDebrisCount = false;
    [SerializeField] private bool useDebrisMultiplier;
    [SerializeField] private AnimationCurve debrisPartsCountMultiplierCurve;

    public event Action OnTriggerSpawn;

    private float startTime;

    void OnEnable()
    {
        if (startOnEnable)
            StartSpawning();
    }
    public void StartSpawning()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnRoutine());
    }
    public IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialWait);

        startTime = Time.time;
        float spawnDelay;
        int similtanousSpawns;
        int debrisCount;

        while (true)
        {
            spawnDelay = (constantSpawnDelay ? spawnDelayRange.y : UnityEngine.Random.Range(spawnDelayRange.x, spawnDelayRange.y))
                * (useSpawnDelayMultiplier ? spawnDelayMultiplierCurve.Evaluate(Time.time - startTime) : 1);

            yield return new WaitForSeconds(spawnDelay);

            similtanousSpawns = useSimiltanousSpawnsMultiplier ? (int)(this.similtanousSpawns * simultaniousSpawnsMultiplierCurve.Evaluate(Time.time - startTime))
                : this.similtanousSpawns;
            debrisCount = (int)((constantDebrisCount ? debrisCountRange.y : UnityEngine.Random.Range(debrisCountRange.x, debrisCountRange.y + 1))
                * (useDebrisMultiplier ? debrisPartsCountMultiplierCurve.Evaluate(Time.time - startTime) : 1));

            TriggerSpawn(similtanousSpawns, debrisCount, overwriteDebrisCount);
        }
    }
    public void TriggerSpawn(int similtanousSpawns, int debrisCount, bool overwriteDebrisCount)
    {
        similtanousSpawns = Mathf.Clamp(similtanousSpawns,0,debrisSpawners.Count());
        debrisSpawners.Shuffle();

        for(int i = 0; i < similtanousSpawns; i++)
        {
            if(overwriteDebrisCount)
                debrisSpawners[i].SpawnDebris(debrisCount);
            else
                debrisSpawners[i].SpawnDebris();
        }
        OnTriggerSpawn?.Invoke();
    }
    [ContextMenu("Get Debris Spawner in Children")]
    private void GetDebrisSpawnerInChildren() =>
        debrisSpawners = GetComponentsInChildren<DebrisSpawner>();
}
