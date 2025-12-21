using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponTrigger))]
public class ChargeStatus : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VehicleControlRoom controlRoom;
    private WeaponTrigger trigger;
    private int teamId;

    public UnityEvent OnCharge = new UnityEvent();
    public UnityEvent OnUncharge = new UnityEvent();

    private bool isPowered;
    public bool IsPowered
    {
        get => isPowered;
        set 
        {
            if (value != isPowered)
            {
                SetChargeStatus(value, IsOvercharged);
            }
        }
    }

    public bool IsOvercharged { get; private set; } = false;

    public bool IsCharged
    {
        get => isPowered || IsOvercharged;    
    }

    private void Awake()
    {
        trigger = GetComponent<WeaponTrigger>();
        teamId = controlRoom.teamId;
    }

    private void OnEnable()
    {
        OverchargedStatus.OnOvercharged += OnOverchargedChanged;
    }

    private void OnDisable()
    {
        OverchargedStatus.OnOvercharged -= OnOverchargedChanged;
    }

    private void OnOverchargedChanged(int team, bool newValue)
    {
        if (teamId == team)
            SetChargeStatus(isPowered, newValue);
    }

    private void SetChargeStatus(bool newPoweredValue, bool newOverchargedValue)
    {
        bool oldChargedState = isPowered || IsOvercharged;
        bool newChargedState = newPoweredValue || newOverchargedValue;

        isPowered = newPoweredValue;
        IsOvercharged = newOverchargedValue;

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
