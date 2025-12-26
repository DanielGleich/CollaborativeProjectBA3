using UnityEngine;

public class WeaponChargeInterface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ChargeStatus weaponCharge;

    [Header("Settings")]
    [SerializeField] LayerMask chargeTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if ((chargeTrigger & (1 << other.gameObject.layer)) != 0)
        {
            weaponCharge.IsPowered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((chargeTrigger & (1 << other.gameObject.layer)) != 0)
        {
            weaponCharge.IsPowered = false;
        }
    }
}
