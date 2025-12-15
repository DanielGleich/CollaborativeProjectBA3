using System;
using UnityEngine;

public class Health : MonoBehaviour
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
            OnUpdateHealth?.Invoke(currentHealth);
            if (currentHealth == 0)
                OnDeath?.Invoke();
        }
    }
    
    public event Action<float> OnUpdateHealth;
    public event Action OnDeath;

    [ContextMenu("DebugTakeDamage")]
    public void DebugDamage()
    {
        CurrentHealth--;
        Debug.Log($"{gameObject.name} => {CurrentHealth}hp");
    }

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }
}