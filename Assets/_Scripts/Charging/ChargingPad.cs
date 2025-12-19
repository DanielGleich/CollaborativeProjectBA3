using UnityEngine;
using UnityEngine.Events;

public class ChargingPad : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ChargingPadTrigger trigger;
    private bool isActive;

    public UnityEvent OnActivate = new UnityEvent();
    public UnityEvent OnDeactivate = new UnityEvent();

    private void OnEnable()
    {
        Subscribe();
        Activate(); //TODO: REMOVE
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        trigger.OnChargeStart += ChargeVehicle;
    }

    private void Unsubscribe()
    { 
        trigger.OnChargeStart -= ChargeVehicle;
    }

    public void Activate()
    { 
        isActive = true;
        OnActivate?.Invoke();
    }

    public void Deactivate()
    {
        isActive = false;
        OnDeactivate?.Invoke();
    }

    private void ChargeVehicle(GameObject vehicle)
    {
        if (isActive)
        {
            OverchargedStatus[] chargingStatuses = vehicle.transform.root.GetComponentsInChildren<OverchargedStatus>();
            foreach (OverchargedStatus chargingStatus in chargingStatuses)
            { 
                chargingStatus.IsOvercharged = true;
            }
        }
    }
}
