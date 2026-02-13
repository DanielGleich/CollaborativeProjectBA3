using FishNet;
using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DebrisSpawnRule
{
    public float healthCondition;
    public int simultaniousSpawns;
    public Vector2Int debrisAmount;
}

/*<summary>
 * The DebrisSpawnManagerHealthExtension is a script 
 * which triggers the DebrisSpawnManager depending on defined 
 * health conditions of the vehicles.
 * </summary>*/

[RequireComponent(typeof(DebrisSpawnManager))]
public class DebrisSpawnManagerHealthExtension : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] List<DebrisSpawnRule> spawnRules;
    List<DebrisSpawnRule> triggeredRules = new List<DebrisSpawnRule>();

    HealthNetworking health;
    DebrisSpawnManager spawner;

    private void OnEnable()
    {
        TeamManager.OnTeamReady += TeamReady;
        if (health != null)
            health.CurrentHealth.OnChange += OnHealthChange;
    }

    private void OnDisable()
    {
        if (health != null)
            health.CurrentHealth.OnChange -= OnHealthChange;
    }


    private void TeamReady(Team team)
    {
        if (IsOwner == false) return;
        if (team.id == TeamMember.localTeamId)
        {
            NetworkObject scientist = TeamManager.Instance?.GetOtherTeamMember(InstanceFinder.ClientManager.Connection.ClientId);
            health = scientist?.transform.GetComponentInChildren<HealthNetworking>();
            if (health != null)
                health.CurrentHealth.OnChange += OnHealthChange;
        }
    }

    private void Start()
    {
        spawner = GetComponent<DebrisSpawnManager>();
        spawnRules.Sort((a, b) => b.healthCondition.CompareTo(a.healthCondition));
    }

    private void OnHealthChange(float prev, float next, bool asServer)
    {
        if (spawnRules.Count <= 0) return;
        DebrisSpawnRule nextRule = spawnRules[0];
        if (next <= nextRule.healthCondition)
        {
            Debug.Log($"{nextRule.healthCondition} - Rule reached");
            spawner.TriggerSpawn(nextRule.simultaniousSpawns, UnityEngine.Random.Range(nextRule.debrisAmount.x, nextRule.debrisAmount.y), true);
            triggeredRules.Add(nextRule);
            spawnRules.Remove(nextRule);
        }
    }
}
