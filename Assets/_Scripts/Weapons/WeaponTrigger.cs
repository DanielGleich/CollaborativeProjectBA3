using System;
using UnityEngine;

[RequireComponent(typeof(ChargeStatus))]
public class WeaponTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Weapon targetWeapon;
    private ChargeStatus chargeStatus;

    public event Action OnTriggered;

    private void Awake()
    {
        chargeStatus = GetComponent<ChargeStatus>();
    }

    public void TriggerWeapon()
    {
        if (chargeStatus.IsCharged)
        {
            targetWeapon.TryActivate();        
        }
    }
}
