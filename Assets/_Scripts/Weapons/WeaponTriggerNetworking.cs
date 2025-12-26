using FishNet.Object;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponTrigger))]
public class WeaponTriggerNetworking : NetworkBehaviour
{
    private WeaponTrigger localTrigger;
    private bool isCooldown = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        localTrigger = GetComponent<WeaponTrigger>();
        localTrigger.Unsubscribe();

        localTrigger.OnTriggerRequest += OnLocalTriggerRequest;
        localTrigger.ControlRoom.OnForceAllWeaponsTrigger += ControlRoom_OnForceAllWeaponsTrigger;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ControlRoom_OnForceAllWeaponsTrigger()
    {
        TriggerWeapon();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnLocalTriggerRequest()
    {
        if (isCooldown == false)
        {
            TriggerWeapon();
            StartCoroutine(Cooldown());
        }
    }

    [ObserversRpc]
    private void TriggerWeapon()
    {
        localTrigger.ForceTriggerWeapon();
    }

    [Server]
    IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(localTrigger.WeaponCooldown);
        isCooldown = false;
    }
}
