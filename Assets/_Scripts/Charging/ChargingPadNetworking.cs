using FishNet.Object;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ChargingPad))]
public class ChargingPadNetworking : NetworkBehaviour
{
    ChargingPad localPad;

    private void Awake()
    {
        localPad = GetComponent<ChargingPad>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(AddPadWhenReady(gameObject));
    }

    [System.Diagnostics.DebuggerHidden] // Für Performance
    private IEnumerator AddPadWhenReady(GameObject pad)
    {
        int attempts = 0;
        while (attempts++ < 10)
        {
            if (ChargingPadManagerNetworking.Instance != null)
            {
                ChargingPadManagerNetworking.Instance.AddChargingPad(pad);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        Debug.LogError("Manager nicht verfügbar!", this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        localPad.Unsubscribe();
        localPad.Trigger.OnCharging += HandleOnChargingEvent;
    }

    private void HandleOnChargingEvent(GameObject obj)
    {
        if (obj.transform.root.TryGetComponent<NetworkObject>(out NetworkObject networkObj))
        {
            if (networkObj.IsOwner)
                RequestChargeServerRpc(networkObj);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestChargeServerRpc(NetworkObject obj)
    {
        if (localPad.IsActive && localPad.IsCooldown == false)
        {
            OverchargedStatus[] chargingStatuses = obj.GetComponentsInChildren<OverchargedStatus>();
            foreach (OverchargedStatus chargingStatus in chargingStatuses)
            {
                if (chargingStatus.IsOvercharged == false)
                {
                    NotifyOverchargeRequest(chargingStatus.GetComponent<NetworkObject>());
                }
            }
        }
    }

    [ObserversRpc]
    private void NotifyOverchargeRequest(NetworkObject obj)
    {
        if (obj.IsOwner)
            obj.GetComponent<OverchargedStatus>().RequestOvercharge();      
        
        if (localPad.TriggerOnce)
            localPad.Deactivate();

        localPad.StartCooldown();
    }
}
