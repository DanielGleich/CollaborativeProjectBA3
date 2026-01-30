using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponTrigger))]
public class ChargeStatus : NetworkBehaviour
{
    [Header("Settings")]
    [field: SerializeField] public int WeaponId { private set; get; } = -1;

    public readonly SyncVar<bool> IsPowered = new SyncVar<bool>();
    public readonly SyncVar<bool> IsOvercharged = new SyncVar<bool>();

    public static event Action<int,int> OnChargeRequest;
    public static event Action<int,int> OnUnchargeRequest;

    public UnityEvent OnChargeActive = new UnityEvent();
    public UnityEvent OnChargeInactive = new UnityEvent();
    public override void OnStartClient()
    {
        base.OnStartClient();

        OnChargeRequest += HandleChargeRequest;
        OnUnchargeRequest += HandleUnchargeRequest;
        OverchargedStatus.OnOvercharged += OnOverchargedChanged;
    }

    public static void ChargeWeapon(int teamId, int weaponId)
    {
        OnChargeRequest?.Invoke(teamId, weaponId);
    }

    public static void UnchargeWeapon(int teamId, int weaponId)
    {
        OnUnchargeRequest?.Invoke(teamId, weaponId);
    }

    public void UnchargeWeapon()
    { 
        OnUnchargeRequest?.Invoke(TeamMember.localTeamId, WeaponId);
    }

    private void HandleChargeRequest(int teamId, int weaponId)
    {
        if (TeamMember.localTeamId == teamId && WeaponId == weaponId )
        {
            Debug.Log(gameObject.transform.root);
            SetChargeStatus(teamId, true);
        }
    }

    private void HandleUnchargeRequest(int teamId, int weaponId)
    {
        if (TeamMember.localTeamId == teamId && WeaponId == weaponId)
            SetChargeStatus(teamId, false);
    }

    private void OnOverchargedChanged(int teamId, bool newValue)
    {
        if (TeamMember.localTeamId == teamId)
            SetOverchargeStatus(teamId, newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetChargeStatus(int teamId, bool newPoweredValue)
    {
        bool oldChargedState = IsPowered.Value || IsOvercharged.Value;
        bool newChargedState = newPoweredValue;

        if (oldChargedState == newChargedState) return;

        IsPowered.Value = newPoweredValue;

        if (oldChargedState == false && newChargedState == true) //Only trigger event, When it was not charged before, but is now charged
        {
            NotifyCharge(teamId);
        }
        else if (oldChargedState == true && newChargedState == false) //Only trigger event, When it was charged before, but is not charged anymore
        {
            NotifyUncharge(teamId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOverchargeStatus(int teamId, bool newOverchargedValue)
    {
        bool oldChargedState = IsPowered.Value || IsOvercharged.Value;
        bool newChargedState = newOverchargedValue;

        if (oldChargedState == newOverchargedValue) return;

        IsOvercharged.Value = newOverchargedValue;

        if (oldChargedState == false && newChargedState == true) //Only trigger event, When it was not charged before, but is now charged
        {
            NotifyCharge(teamId);
        }
        else if (oldChargedState == true && newChargedState == false) //Only trigger event, When it was charged before, but is not charged anymore
        {
            NotifyUncharge(teamId);
        }
    }

    [ObserversRpc]
    void NotifyCharge(int teamId)
    {
        OnChargeActive?.Invoke();
    }

    [ObserversRpc]
    void NotifyUncharge(int teamId)
    {
        OnChargeInactive?.Invoke();
    }
}
