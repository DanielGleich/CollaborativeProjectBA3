using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [Header("Settings")]
    [field: SerializeField] public float MaxHealth { get; private set; }
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            value = Mathf.Clamp(value, 0, MaxHealth);
            if (value == currentHealth)
                return;
            currentHealth = value;
            OnChangeHealth?.Invoke(currentHealth);
            if (currentHealth == 0)
                OnDeath?.Invoke();
        }
    }

    public event Action<Damage> OnDamaged;
    public event Action<float> OnChangeHealth;
    public event Action OnDeath;

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public virtual void TakeDamage(Damage damage)
    {
        OnDamaged?.Invoke(damage);
        CurrentHealth -= damage.DamageAmount;
    }
}