using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;


/*<summary>
 * The WeaponChargeInterface is the legacy version of our weapon chargers within the rat vehicle.
 * When the a physic object of the LayerMask chargeTrigger is in contact with this gameobject the weapon with the weaponid 
 * becomes charged for scientist.
 * </summary>*/

public class WeaponChargeInterface : NetworkBehaviour
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
        if (IsOwner && (chargeTrigger & (1 << other.gameObject.layer)) != 0)
        {
            ChargeStatus.ChargeWeapon(TeamMember.localTeamId, weaponId);
            OnChargeTrigger?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsOwner && (chargeTrigger & (1 << other.gameObject.layer)) != 0)
        {
            ChargeStatus.UnchargeWeapon(TeamMember.localTeamId, weaponId);
            OnUnchargeTrigger?.Invoke();
        }
    }
}
