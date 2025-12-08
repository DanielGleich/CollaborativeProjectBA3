using System;
using UnityEngine;

public abstract class HealthDisplay : MonoBehaviour {
    [Header("References")]
    [SerializeField] protected Health health;

    protected virtual void OnEnable()
    {
        health.OnChangeHealth += UpdateHealthDisplay;
        UpdateHealthDisplay(health.CurrentHealth);
    }
    protected virtual void OnDisable()
    {
        health.OnChangeHealth += UpdateHealthDisplay;
    }
    
    protected abstract void UpdateHealthDisplay(float currentHealth);
}