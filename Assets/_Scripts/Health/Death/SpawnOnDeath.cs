using UnityEngine;

public class SpawnOnDeath : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private GameObject deathPrefab;

    void OnEnable() => health.OnDeath += OnDeath;
    void OnDisable() => health.OnDeath -= OnDeath;
    
    void OnValidate()
    {
        if (!health)
            health = GetComponent<Health>();
    }

    private void OnDeath()
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
    }
}