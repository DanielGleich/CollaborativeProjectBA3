using System;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// Applies force and optionaly despawns projectile OnCollision/ OnTriggerEnter
/// </summary>
public class NetworkedProjectile : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    [Header("Settings")]
    [SerializeField] private float spawnVelocity;
    [SerializeField] private bool despawnOnHit = true;

    public event Action OnHit;

    public override void OnStartClient()
    {
        base.OnStartClient();
        rb.AddForce(transform.forward * spawnVelocity, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision collision)
    {
        OnHit?.Invoke();
        if (IsOwner && despawnOnHit)
            Despawn();
        gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        OnHit?.Invoke();
        if (IsOwner && despawnOnHit)
            Despawn();
        gameObject.SetActive(false);
    }
}