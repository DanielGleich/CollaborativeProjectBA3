using UnityEngine;

/// <summary>
/// Spawns a hit feedback Prefab in case the projectile hits something
/// </summary>
public class ProjectileHitFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkedProjectile networkedProjectile;
    [SerializeField] private GameObject projectileHitFeedbackPrefab;

    void OnEnable()
    {
        networkedProjectile.OnHit += TriggerHitFeedback;
    }
    void OnDisable()
    {
        networkedProjectile.OnHit -= TriggerHitFeedback;
    }

    private void TriggerHitFeedback()
    {
        Instantiate(projectileHitFeedbackPrefab, networkedProjectile.transform.position, networkedProjectile.transform.rotation);
    }
}
