using System;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthNetworking networkedHealth;
    [SerializeField, Tooltip("Position of the gameobject replaced by the death prefab")] private Transform spawnPosition;
    [SerializeField] private GameObject deathPrefab;

    void OnEnable()
    {
        networkedHealth.OnNetworkedDeath += HandleDeath;
    }
    void OnDisable()
    {
        networkedHealth.OnNetworkedDeath -= HandleDeath;
    }
    private void HandleDeath()
    {
        Instantiate(deathPrefab, spawnPosition.position, spawnPosition.rotation);
    }

}
