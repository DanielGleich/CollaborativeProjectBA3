using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies constant damage to any health component, that is in the triggerzone of this object
/// Tipp: Use the IgnorCollisions3d component to not damage yourself
/// </summary>
public class ConstantDamage : SimpleDamage {
    [SerializeField, Min(0)] private float damagePerSecond = 1f;
    private List<Health> affectedHealthComponents = new();
    protected override void OnTriggerEnter(Collider other)
    {
        if (enabled)
            CheckNewContacts(other);
    }

    protected void OnTriggerStay(Collider other)
    {
        if (enabled)
            CheckNewContacts(other);
    }

    void CheckNewContacts(Collider other)
    {
        Health h = other.GetComponentInParent<Health>();
        if (h && !affectedHealthComponents.Contains(h))
        {
            affectedHealthComponents.Add(h);
            TakeDamage(h);
        }
    }
    void OnTriggerExit(Collider other)
    {
        Health h = other.GetComponentInParent<Health>();
        if(h && affectedHealthComponents.Contains(h))
        {
            affectedHealthComponents.Remove(h);
        }
    }
    void OnDisable()
    {
        affectedHealthComponents.Clear();
    }
    void Update()
    {
        foreach (Health h in affectedHealthComponents)
        {
            var networkOwnerShipGuard = GetComponentInParent<DamageNetworking>();
            if (networkOwnerShipGuard?.CanApplyDamage(gameObject) == false)
                continue;
            h.CurrentHealth -= damagePerSecond * Time.deltaTime;
        }
    }
}