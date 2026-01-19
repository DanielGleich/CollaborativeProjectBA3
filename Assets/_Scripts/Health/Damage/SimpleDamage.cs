using System;
using UnityEngine;

/// <summary>
/// Applies damage to any health component, that enters the triggerzone of this object
/// </summary>
public class SimpleDamage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] protected float damageOnEnter = 0f;

    public event Action OnHit;

    protected virtual void OnTriggerEnter(Collider other)
    {
        Health h = other.GetComponentInParent<Health>();
        if (!h)
            return;
        TakeDamage(h);
    }
    protected void TakeDamage(Health healthComponent)
    {
        var networkOwnerShipGuard = GetComponentInParent<DamageNetworking>();
        if (networkOwnerShipGuard?.CanApplyDamage(gameObject) == false)
            return;
        healthComponent.CurrentHealth -= damageOnEnter;
        OnHit?.Invoke();
    }
}