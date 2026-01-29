using UnityEngine;

public class ProjectileTrailFeedback : MonoBehaviour
{
    [SerializeField] NetworkedProjectile projectile;
    [SerializeField] ParticleSystem pSystem;
    [SerializeField] SelfDestroyWithDelay selfdestroy;

    private void OnEnable()
    {
        projectile.OnHit += ProjectileTrailFeedback_OnHit;
    }

    private void OnDisable()
    {
        projectile.OnHit -= ProjectileTrailFeedback_OnHit;
    }

    private void ProjectileTrailFeedback_OnHit()
    {
        gameObject.transform.SetParent(null, true);
        pSystem.Stop();
        selfdestroy.Trigger();
    }
}
