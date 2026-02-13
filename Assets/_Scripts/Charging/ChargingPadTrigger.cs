using System;
using UnityEngine;

/*<summary>
 * The ChargingPadTrigger is part of the ChargingPads. 
 * When a vehicle enters this trigger a signal gets forwarded to the ChargingPad script.
 * </summary>*/

public class ChargingPadTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask VehicleMask;

    public event Action<GameObject> OnChargeStart;
    public event Action<GameObject> OnCharging;
    public event Action<GameObject> OnChargeStop;

    private void OnTriggerEnter(Collider other)
    {
        if ((VehicleMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnChargeStart?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((VehicleMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnCharging?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((VehicleMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnChargeStop?.Invoke(other.gameObject);
        }
    }
}
