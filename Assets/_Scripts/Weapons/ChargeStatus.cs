using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponTrigger))]
public class ChargeStatus : MonoBehaviour
{
    [Header("Settings")]
    [field: SerializeField] public int WeaponId { private set; get; } = -1;

    public static event Action<int,int> OnCharge;
    public static event Action<int,int> OnUncharge;

    public UnityEvent OnChargeActive = new UnityEvent();
    public UnityEvent OnChargeInactive = new UnityEvent();

    public bool IsPowered { get; private set; } = false;
    public bool IsOvercharged { get; private set; } = false;

    public bool IsCharged
    {
        get => IsPowered || IsOvercharged;    
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    { 
        OverchargedStatus.OnOvercharged += OnOverchargedChanged;
        ChargeStatus.OnCharge += OnCharged;
        ChargeStatus.OnUncharge += OnUncharged;        
    }

    public void Unsubscribe()
    { 
        OverchargedStatus.OnOvercharged -= OnOverchargedChanged;
        ChargeStatus.OnCharge -= OnCharged;
        ChargeStatus.OnUncharge -= OnUncharged;        
    }

    public static void ChargeWeapon(int teamId, int weaponId)
    {
        OnCharge?.Invoke(teamId, weaponId);
    }

    public static void UnchargeWeapon(int teamId, int weaponId)
    {
        OnUncharge?.Invoke(teamId, weaponId);
    }

    private void OnCharged(int teamId, int weaponId)
    {
        if (TeamMember.localTeamId == teamId && this.WeaponId == weaponId)
            SetChargeStatus(true, IsOvercharged);
    }

    private void OnUncharged(int teamId, int weaponId)
    {
        if (TeamMember.localTeamId == teamId && this.WeaponId == weaponId)
            SetChargeStatus(false, IsOvercharged);
    }

    private void OnOverchargedChanged(int teamId, bool newValue)
    {
        if (TeamMember.localTeamId == teamId)
            SetChargeStatus(IsPowered, newValue);
    }

    private void SetChargeStatus(bool newPoweredValue, bool newOverchargedValue)
    {
        bool oldChargedState = IsPowered || IsOvercharged;
        bool newChargedState = newPoweredValue || newOverchargedValue;

        IsPowered = newPoweredValue;
        IsOvercharged = newOverchargedValue;

        if (oldChargedState == false && newChargedState == true) //Only trigger event, When it was not charged before, but is now charged
        {
            OnChargeActive?.Invoke();
        }
        else if (oldChargedState == true && newChargedState == false) //Only trigger event, When it was charged before, but is not charged anymore
        {
            OnChargeInactive?.Invoke();
        }
    }
}
