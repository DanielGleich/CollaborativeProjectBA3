using UnityEngine;

public class SpawnOnDeath : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private GameObject deathPrefab;

    void OnEnable() => health.OnDeath += SpawnDeathPrefab;
    void OnDisable() => health.OnDeath -= SpawnDeathPrefab;
    
    void OnValidate()
    {
        if (!health)
            health = GetComponent<Health>();
    }

    private void SpawnDeathPrefab()
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
    }
}