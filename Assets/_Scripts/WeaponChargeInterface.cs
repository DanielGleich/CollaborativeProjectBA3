using UnityEngine;
using UnityEngine.Events;

public class WeaponChargeInterface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] int weaponId = -1;

    [Header("Settings")]
    [SerializeField] LayerMask chargeTrigger;

    [Header("Events")]
    public UnityEvent OnChargeTrigger = new UnityEvent();
    public UnityEvent OnUnchargeTrigger = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if ((chargeTrigger & (1 << other.gameObject.layer)) != 0)
        {
            ChargeStatus.ChargeWeapon(TeamMember.localTeamId, weaponId);
            OnChargeTrigger?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((chargeTrigger & (1 << other.gameObject.layer)) != 0)
        {
            ChargeStatus.UnchargeWeapon(TeamMember.localTeamId, weaponId);
            OnUnchargeTrigger?.Invoke();
        }
    }
}
