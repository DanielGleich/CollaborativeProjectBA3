using UnityEngine;

public class SimpleDamage : Damage
{
    [Header("Settings")]
    [SerializeField, Min(0)] private float damageAmount;
    [SerializeField, Min(0)] private float knockBackForce;
    
    public override float DamageAmount => damageAmount;
    public override float KnockbackForce => knockBackForce;
}