using FishNet.Object;
using UnityEngine;

public class StageHazardManager : NetworkSingleton<StageHazardManager>
{
    protected override bool _perClient { get; } = false;

    [ServerRpc(RequireOwnership = false)]
    public void RequestTrigger()
    {
        HandleNetworkedTrigger();
    }

    [ObserversRpc]
    private void HandleNetworkedTrigger()
    {
        StageHazard.TriggerAllHazards();
    }
}
