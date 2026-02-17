using FishNet.Connection;
using System;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthNetworking networkedHealth;
    [SerializeField, Tooltip("Position of the gameobject replaced by the death prefab")] private Transform spawnPosition;
    [SerializeField] private GameObject deathPrefab;

    [Header("Settings")]
    [SerializeField] private bool destroyOnDeath = true;

    void OnValidate()
    {
        if(!networkedHealth)
            networkedHealth = GetComponent<HealthNetworking>();
        if(!spawnPosition)
            spawnPosition = transform;
    }

    void OnEnable()
    {
        networkedHealth.OnNetworkedDeath += HandleDeath;
    }
    void OnDisable()
    {
        networkedHealth.OnNetworkedDeath -= HandleDeath;
    }
    private void HandleDeath(NetworkConnection c)
    {
        Instantiate(deathPrefab, spawnPosition.position, spawnPosition.rotation);
        if(destroyOnDeath)
            Destroy(gameObject);
    }

}
