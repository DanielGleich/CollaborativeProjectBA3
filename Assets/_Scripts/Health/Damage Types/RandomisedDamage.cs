using UnityEngine;

public class RandomisedDamage : Damage
{
    [Header("Settings")]
    [SerializeField, Min(0)] private Vector2 damageAmount;
    [SerializeField, Min(0)] private float knockBackForce;
    
    public override float DamageAmount => Random.Range(damageAmount.x, damageAmount.y);
    public override float KnockbackForce => knockBackForce;
}