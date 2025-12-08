using System;
using UnityEngine;

public class SimpleDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    void OnEnable() => health.OnDeath += OnDeath;
    void OnDisable() => health.OnDeath -= OnDeath;

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}