using FishNet.Object;
using System;
using UnityEngine;

[RequireComponent(typeof(StageHazard))]
public class StageHazardNetworking : NetworkBehaviour
{
    public static event Action OnNetworkedTrigger;

    private void OnEnable()
    {
        StageHazard.OnTriggered += RequestNetworkedTrigger;
    }
    private void OnDisable()
    {
        StageHazard.OnTriggered -= RequestNetworkedTrigger;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestNetworkedTrigger()
    {
        TriggerNetworkedTrigger();
    }

    [ObserversRpc]
    private void TriggerNetworkedTrigger()
    {
        OnNetworkedTrigger?.Invoke();
    }
}
