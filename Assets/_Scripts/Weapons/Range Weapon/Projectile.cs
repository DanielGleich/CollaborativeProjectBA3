using System;
using FishNet.Object;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private float spawnVelocity;
    [SerializeField] private bool despawnOnHit = true;

    public event Action<Collision> OnHit;

    public override void OnStartClient()
    {
        base.OnStartClient();
        rb.AddForce(transform.forward * spawnVelocity, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision collision)
    {
        OnHit?.Invoke(collision);
        if(despawnOnHit)
            Despawn(gameObject);
    }
}