using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

[RequireComponent(typeof(ChargeStatus))]
public class ChargeStatusNetworking : NetworkBehaviour
{
    public ChargeStatus localChargeStatus { get; private set; }
    public readonly SyncVar<bool> IsPowered = new SyncVar<bool>();
    public readonly SyncVar<bool> IsOvercharged = new SyncVar<bool>();
    private void Awake()
    {
        localChargeStatus = GetComponent<ChargeStatus>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        localChargeStatus.Unsubscribe();

        OverchargedStatus.OnOvercharged += OnOverchargedChanged;
        ChargeStatus.OnCharge += HandleChargeRequest;
        ChargeStatus.OnUncharge += HandleUnchargeRequest;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        OverchargedStatus.OnOvercharged -= OnOverchargedChanged;
        ChargeStatus.OnCharge -= HandleChargeRequest;
        ChargeStatus.OnUncharge -= HandleUnchargeRequest;
    }

    private void HandleChargeRequest(int teamId, int weaponId)
    {
        if (TeamMember.localTeamId == teamId && localChargeStatus.WeaponId == weaponId)
            SetChargeStatus(teamId, true, IsOvercharged.Value);
    }

    private void HandleUnchargeRequest(int teamId, int weaponId)
    {
        if (TeamMember.localTeamId == teamId && localChargeStatus.WeaponId == weaponId)
            SetChargeStatus(teamId, false, IsOvercharged.Value);
    }

    private void OnOverchargedChanged(int teamId, bool newValue)
    {
        if (TeamMember.localTeamId == teamId)
            SetChargeStatus(teamId, IsPowered.Value, newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetChargeStatus(int teamId, bool newPoweredValue, bool newOverchargedValue)
    {
        bool oldChargedState = IsPowered.Value || IsOvercharged.Value;
        bool newChargedState = newPoweredValue || newOverchargedValue;

        IsPowered.Value = newPoweredValue;
        IsOvercharged.Value = newOverchargedValue;

        if (oldChargedState == false && newChargedState == true) //Only trigger event, When it was not charged before, but is now charged
        {
            NotifyCharge(teamId);
            Debug.Log(gameObject + " charged");
        }
        else if (oldChargedState == true && newChargedState == false) //Only trigger event, When it was charged before, but is not charged anymore
        {
            NotifyUncharge(teamId);
        }
    }

    [ObserversRpc]
    void NotifyCharge(int teamId)
    {
        localChargeStatus.OnChargeActive?.Invoke();
    }

    [ObserversRpc]
    void NotifyUncharge(int teamId)
    {
        localChargeStatus.OnChargeInactive?.Invoke();
    }
}
