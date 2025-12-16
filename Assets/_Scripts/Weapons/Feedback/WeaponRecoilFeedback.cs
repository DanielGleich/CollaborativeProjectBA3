using UnityEngine;

/// <summary>
/// Triggers Knockback, when a weapon is activated
/// </summary>
public class WeaponRecoilFeedback : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private Rigidbody affectedRigidbody;
    [SerializeField] private Transform weaponDirection;

    [Header("Settings")]
    [SerializeField] private float knockBackForce;

    void OnEnable()
    {
        weapon.OnActivate += InvokeKnockback;
    }
    void OnDisable()
    {
        weapon.OnActivate -= InvokeKnockback;
    }
    private void InvokeKnockback(bool activated)
    {
        if(activated)
            affectedRigidbody.AddForce(-weaponDirection.forward * knockBackForce, ForceMode.Impulse);
    }
}