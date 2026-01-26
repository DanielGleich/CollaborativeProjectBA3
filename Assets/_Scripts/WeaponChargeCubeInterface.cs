using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BatteryUseField))]
public class WeaponChargeCubeInterface : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] int weaponId = -1;

    BatteryUseField batteryUseField;

    List<Battery> batteries = new List<Battery>();

    [Header("Events")]
    public UnityEvent OnChargeTrigger = new UnityEvent();
    public UnityEvent OnUnchargeTrigger = new UnityEvent();

    private void Awake()
    {
        batteryUseField = GetComponent<BatteryUseField>();
    }

    private void OnEnable()
    {
        WeaponTriggerNetworking.OnWeaponTriggered += OnChargeUsed;
    }

    private void OnDisable()
    {
        WeaponTriggerNetworking.OnWeaponTriggered -= OnChargeUsed;
    }

    private void OnChargeUsed(int teamId, int triggeredWeaponId)
    {
        if (IsOwner && TeamMember.localTeamId == teamId && triggeredWeaponId == weaponId)
            batteryUseField.UseUpBatteries();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner && other.gameObject.TryGetComponent<Battery>(out Battery bat) && batteries.Contains(bat) == false)
        {
            batteries.Add(bat);
            if (batteries.Count >= batteryUseField.requiredChargedBatteries)
            {
                ChargeStatus.ChargeWeapon(TeamMember.localTeamId, weaponId);
                OnChargeTrigger?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsOwner && other.gameObject.TryGetComponent<Battery>(out Battery bat) && batteries.Contains(bat))
        {
            batteries.Remove(bat);
            if (batteries.Count < batteryUseField.requiredChargedBatteries)
            {
                ChargeStatus.UnchargeWeapon(TeamMember.localTeamId, weaponId);
                OnUnchargeTrigger?.Invoke();
            }
        }
    }
}
