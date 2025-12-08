using UnityEngine;

public class PhysicsDamage : Damage {
    [SerializeField] private Rigidbody rb;
    [SerializeField, Min(0)] private float noDamageRange = 3; 
    [SerializeField, Min(0)] private float forceMultiplier = 1;
    [SerializeField, Min(0)] private Vector2 damageRange = new Vector2(0, 100);

    private float collisionDamage;

    public override float DamageAmount => collisionDamage;

    public override float KnockbackForce => collisionDamage;

    protected override void OnCollisionEnter(Collision collision)
    {
        collisionDamage = Mathf.Clamp(Mathf.Max(collision.relativeVelocity.magnitude - noDamageRange, 0) * forceMultiplier, damageRange.x, damageRange.y);
        if (collisionDamage == 0)
            return;
        base.OnCollisionEnter(collision);
    }
}