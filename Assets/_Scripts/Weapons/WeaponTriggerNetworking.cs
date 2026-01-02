using FishNet.Object;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponTrigger))]
public class WeaponTriggerNetworking : NetworkBehaviour
{
    private WeaponTrigger localTrigger;
    private ChargeStatusNetworking networkedChargeStatus;
    private bool isCooldown = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        localTrigger = GetComponent<WeaponTrigger>();
        localTrigger.Unsubscribe();
        networkedChargeStatus = GetComponent<ChargeStatusNetworking>();

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
        if (isCooldown == false && (networkedChargeStatus.IsPowered.Value || networkedChargeStatus.IsOvercharged.Value))
        {
            TriggerWeapon();
            StartCoroutine(Cooldown());
            OverchargedStatus.RequestUseOvercharge(teamId);
        }
    }

    [ObserversRpc]
    private void TriggerWeapon()
    {
        if (IsOwner)
        {
            localTrigger.ForceTriggerWeapon();
        }
        else
        {
            localTrigger.ForceTriggerWeaponAnimation();
        }
    }

    [Server]
    IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(localTrigger.WeaponCooldown);
        isCooldown = false;
    }
}
