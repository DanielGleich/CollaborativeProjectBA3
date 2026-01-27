using FMODUnity;
using UnityEngine;

public class NetworkHitFeedback : MonoBehaviour {
    [Header("References")]
    [SerializeField] private HealthNetworking networkedHealth;
    [SerializeField] private EventReference selfHit, enemyHit;

    void OnEnable()
    {
        networkedHealth.CurrentHealth.OnChange += GiveHitFeedback;
    }
    void OnDisable()
    {
        networkedHealth.CurrentHealth.OnChange -= GiveHitFeedback;
    }
    private void GiveHitFeedback(float prev, float next, bool asServer)
    {
        // If health stays the same or increases don't trigger hit feedback 
        if(prev <= next)
            return;
        // Play a diffent sound depending on if the player or the enemy was hit
        RuntimeManager.PlayOneShot(networkedHealth.IsOwner? selfHit : enemyHit, transform.position);
    }
}