using UnityEngine;

/// <summary>
/// Triggers Knockback, when a weapon is activated
/// </summary>
public class WeaponRecoilFeedback : WeaponFeedback
{
    [SerializeField] private Rigidbody affectedRigidbody;
    [SerializeField] private Transform weaponDirection;

    [Header("Settings")]
    [SerializeField] private float knockBackForce;

    protected override void WeaponActivated(bool isActivated)
    {
        if (isActivated)
            affectedRigidbody.AddForceAtPosition(-weaponDirection.forward, weapon.transform.position, ForceMode.Impulse);
    }
}