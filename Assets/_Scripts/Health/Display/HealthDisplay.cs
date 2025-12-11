using System;
using UnityEngine;

public abstract class HealthDisplay : MonoBehaviour {
    [Header("References")]
    [SerializeField] protected Health health;

    void OnEnable()
    {
        health.OnUpdateHealth += UpdatehealthDisplay;
    }
    void OnDisable()
    {
        health.OnUpdateHealth += UpdatehealthDisplay;
    }
    protected abstract void UpdatehealthDisplay(float health);
}