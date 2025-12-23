using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponTrigger))]
public class ChargeStatus : MonoBehaviour
{
    [Header("References")]
    [SerializeField] OverchargedStatus vehicleOverchargedStatus;
    private WeaponTrigger trigger;
    private bool isPowered;
    public bool IsPowered
    {
        get => isPowered;
        private set 
        {
            if (value != isPowered)
            {
                SetChargeStatus(value, isOvercharged);
            }
        }
    }

    private bool isOvercharged;

    public bool IsCharged
    {
        get => isPowered || isOvercharged;    
    }

    public UnityEvent OnCharge = new UnityEvent();
    public UnityEvent OnUncharge = new UnityEvent();

    private void Awake()
    {
        trigger = GetComponent<WeaponTrigger>();
    }

    private void OnEnable()
    {
        vehicleOverchargedStatus.OnOverchargedChanged += OnOverchargedChanged;
        trigger.OnTriggered += UseOvercharge;
    }

    private void OnDisable()
    {
        vehicleOverchargedStatus.OnOverchargedChanged -= OnOverchargedChanged;
        trigger.OnTriggered -= UseOvercharge;
    }

    private void OnOverchargedChanged(bool isOvercharging)
    {
        if (isOvercharging)
            SetChargeStatus(isPowered, isOvercharging);
    }

    private void UseOvercharge()
    {
        SetChargeStatus(isPowered, false);
    }

    private void SetChargeStatus(bool newPoweredValue, bool newOverchargedValue)
    {
        bool oldChargedState = isPowered || isOvercharged;
        bool newChargedState = newPoweredValue || newOverchargedValue;

        IsPowered = newPoweredValue;
        isOvercharged = newOverchargedValue;

        if (oldChargedState == false && newChargedState == true) //Only trigger event, When it was not charged before, but is now charged
        {
            OnCharge?.Invoke();
        }
        else if (oldChargedState == true && newChargedState == false) //Only trigger event, When it was charged before, but is not charged anymore
        { 
            OnUncharge?.Invoke();
        }
    }
}
