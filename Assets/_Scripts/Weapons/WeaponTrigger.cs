using FishNet.Object;
using System;
using System.Collections;
using UnityEngine;

public class WeaponTrigger : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Weapon targetWeapon;
    private ChargeStatus chargeStatus;

    [Header("Settings")]
    [field: SerializeField] public float WeaponCooldown { private set; get; } = 0;
    [field: SerializeField] public bool GodMode { private set; get; } = false;

    public bool IsCooldown { get; private set; } = false;
    int weaponId = -1;


    public static event Action<int, int> OnWeaponTriggered;

    public override void OnStartClient()
    {
        base.OnStartClient();
        chargeStatus = GetComponent<ChargeStatus>();
        weaponId = chargeStatus.WeaponId;

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
        if (GodMode || chargeStatus.IsOvercharged.Value)
        {
            ForceTriggerWeapon(teamId);
            StartCoroutine(Cooldown());
        }
        else if (IsCooldown == false && chargeStatus.IsPowered.Value)
        {
            NotifyWeaponTrigger(teamId);
            chargeStatus.UnchargeWeapon();
            StartCoroutine(Cooldown());
        }
    }

    [Server]
    public void ForceTriggerWeapon(int teamId)
    {
        StopAllCoroutines();
        NotifyWeaponTrigger(teamId);
        StartCoroutine(Cooldown());
    }

    public void TriggerWeaponAnimation()
    {
        //Probably needs to be changed if damage is dealt double/quadruple
        targetWeapon.TryActivate();
    }

    [ObserversRpc]
    private void NotifyWeaponTrigger(int teamId)
    {
        if (IsOwner && teamId == TeamMember.localTeamId)
        {
            targetWeapon.TryActivate();
        }
        else
        {
            TriggerWeaponAnimation();
        }
        OnWeaponTriggered?.Invoke(teamId, weaponId);
    }

    [Server]
    IEnumerator Cooldown()
    {
        IsCooldown = true;
        yield return new WaitForSeconds(WeaponCooldown);
        IsCooldown = false;
    }
}
