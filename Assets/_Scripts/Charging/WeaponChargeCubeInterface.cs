using FishNet.Object;
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
        WeaponTrigger.OnWeaponTriggered += OnWeaponTriggered;
    }

    private void OnDisable()
    {
        WeaponTrigger.OnWeaponTriggered -= OnWeaponTriggered;
    }

    private void OnWeaponTriggered(int teamId, int triggeredWeaponId)
    {
        if (IsOwner && TeamMember.localTeamId == teamId && triggeredWeaponId == weaponId)
        {
            batteryUseField.UseUpBatteries();
            if (batteryUseField.IsReady == false)
            {
                OnUnchargeTrigger?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner && other.gameObject.TryGetComponent<Battery>(out Battery bat))
        {
            if (batteryUseField.IsReady)
            {
                if (IsOwner)
                    Debug.Log($"Team {TeamMember.localTeamId} charged by local client");
                ChargeStatus.ChargeWeapon(TeamMember.localTeamId, weaponId);
                OnChargeTrigger?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsOwner && other.gameObject.TryGetComponent<Battery>(out Battery bat))
        {
            if (batteryUseField.IsReady)
            {
                if (IsOwner)
                    Debug.Log($"Team {TeamMember.localTeamId} charged by local client");
                ChargeStatus.UnchargeWeapon(TeamMember.localTeamId, weaponId);
                OnUnchargeTrigger?.Invoke();
            }
        }
    }
}
