using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BatteryUseField))]
public class WeaponChargeCubeInterface : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] int weaponId = -1;

    BatteryUseField batteryUseField;

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
        {
            batteryUseField.UseUpBatteries();
            if (batteryUseField.batteries.Count <= batteryUseField.requiredChargedBatteries)
            {
                ChargeStatus.UnchargeWeapon(TeamMember.localTeamId, weaponId);
                OnUnchargeTrigger?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner && other.gameObject.TryGetComponent<Battery>(out Battery bat))
        {
            if (batteryUseField.batteries.Count >= batteryUseField.requiredChargedBatteries)
            {
                ChargeStatus.ChargeWeapon(TeamMember.localTeamId, weaponId);
                OnChargeTrigger?.Invoke();
                Debug.Log($"{gameObject} charged");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsOwner && other.gameObject.TryGetComponent<Battery>(out Battery bat))
        {
            if (batteryUseField.batteries.Count < batteryUseField.requiredChargedBatteries)
            {
                ChargeStatus.UnchargeWeapon(TeamMember.localTeamId, weaponId);
                OnUnchargeTrigger?.Invoke();
                Debug.Log($"{gameObject} uncharged");
            }
        }
    }
}
