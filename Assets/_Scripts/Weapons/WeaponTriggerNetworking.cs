using FishNet.Object;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponTrigger))]
public class WeaponTriggerNetworking : NetworkBehaviour
{
    [Header("Options")]
    [field: SerializeField] public bool GodMode { private set; get; } = false;
    
    private bool isCooldown = false;
    int weaponId = -1;
    
    private ChargeStatusNetworking networkedChargeStatus;
    private WeaponTrigger localTrigger;

    public static event Action<int,int> OnWeaponTriggered;

    public override void OnStartClient()
    {
        base.OnStartClient();
        localTrigger = GetComponent<WeaponTrigger>();
        localTrigger.Unsubscribe();
        networkedChargeStatus = GetComponent<ChargeStatusNetworking>();
        weaponId = networkedChargeStatus.localChargeStatus.WeaponId;

        if (IsOwner)
            WeaponManager.OnWeaponTrigger += HandleLocalTriggerRequest;
    }

    void HandleLocalTriggerRequest(int teamId)
    {
        if (TeamMember.localTeamId == teamId && IsOwner)
        { 
            OnLocalTriggerRequest(teamId);
        }
    }

    [ServerRpc]
    private void OnLocalTriggerRequest(int teamId)
    {

        if ( GodMode || networkedChargeStatus.IsOvercharged.Value || (isCooldown == false && networkedChargeStatus.IsPowered.Value))
        {
            TriggerWeapon(teamId);
            StartCoroutine(Cooldown());
            OverchargedStatus.RequestUseOvercharge(teamId);
        }
    }

    [ObserversRpc]
    private void TriggerWeapon(int teamId)
    {
        if (IsOwner)
        {
            localTrigger.ForceTriggerWeapon();
        }
        else
        {
            localTrigger.ForceTriggerWeaponAnimation();
        }
        OnWeaponTriggered?.Invoke(teamId, weaponId);
    }

    [Server]
    IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(localTrigger.WeaponCooldown);
        isCooldown = false;
    }
}
