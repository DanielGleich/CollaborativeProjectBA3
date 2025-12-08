using System;
using UnityEngine;

public abstract class Damage : MonoBehaviour
{
    public abstract float DamageAmount { get; }
    public abstract float KnockbackForce { get; }
    public event Action OnHitDamagable;

    protected virtual void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponentInParent<IDamagable>();
        if (damageable == null)
            return;
        damageable.TakeDamage(this);
        OnHitDamagable?.Invoke();
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        var damageable = collision.collider.GetComponentInParent<IDamagable>();
        if (damageable == null)
            return;
        damageable.TakeDamage(this);
        OnHitDamagable?.Invoke();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponentInParent<IDamagable>();
        if (damageable == null)
            return;
        damageable.TakeDamage(this);
        OnHitDamagable?.Invoke();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var damageable = collision.collider.GetComponentInParent<IDamagable>();
        if (damageable == null)
            return;
        damageable.TakeDamage(this);
        OnHitDamagable?.Invoke();
    }
}