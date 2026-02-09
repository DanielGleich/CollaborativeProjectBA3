using FishNet.Connection;
using UnityEngine;

public class OnDeathSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HealthNetworking networkedHealth;
    [SerializeField] GameObject prefab;

    [Header("Settings")]
    [SerializeField] bool triggerOnce = true;

    bool isTriggered = false;

    private void OnEnable()
    {
        //if (localHealth != null)
        //    localHealth.OnDeath += Spawn;
        if (networkedHealth != null)
            networkedHealth.OnNetworkedDeath += Spawn;
    }
    private void OnDisable()
    {
        //if (localHealth != null)
        //    localHealth.OnDeath -= Spawn;
        if (networkedHealth != null)
            networkedHealth.OnNetworkedDeath -= Spawn;
    }

    private void Spawn()
    {
        if (triggerOnce && isTriggered) return;
        Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
        isTriggered = true;
    }

    private void Spawn(NetworkConnection c)
    {
        Spawn();
    }
}
