using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(ChainsawTrap))]
public class ChainsawTrapNetworking : NetworkBehaviour
{
    ChainsawTrap localTrap;

    private void Awake()
    {
        localTrap = GetComponent<ChainsawTrap>();
    }

    private void Subscribe()
    { 
        localTrap.Unsubscribe();
        StageHazardNetworking.OnNetworkedTrigger += NetworkedTriggerTrap;
    }
    private void Unsubscribe()
    { 
        localTrap.Subscribe();
        StageHazardNetworking.OnNetworkedTrigger -= NetworkedTriggerTrap;
    }

    private void NetworkedTriggerTrap()
    {
        localTrap.TriggerTrap();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Subscribe();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Unsubscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
