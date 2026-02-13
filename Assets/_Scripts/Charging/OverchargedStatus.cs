using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;

/*<summary>
 * OverchargedStatus is an additional status for the vehicle which makes it
 * possible to trigger all weapons at the same time and ignore the usual 
 * weapon cooldowns. Overcharging is possible with ChargingPads.
 * </summary>*/

public class OverchargedStatus : NetworkBehaviour
{
    public static event Action<int> OnUseOverchargeRequest;
    public static event Action<int, bool> OnOvercharged;

    public readonly SyncVar<bool> IsOvercharged = new SyncVar<bool>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
            OnUseOverchargeRequest += RequestOverchargeUse;
    }

    public void RequestOvercharge()
    {
        SetOverchargeServerRpc(TeamMember.localTeamId);
    }
    public static void RequestUseOvercharge(int teamId)
    {
        OnUseOverchargeRequest?.Invoke(teamId);
    }

    public void RequestUseOvercharge()
    { 
        OnUseOverchargeRequest(TeamMember.localTeamId);
    }

    [ServerRpc]
    private void SetOverchargeServerRpc(int teamId)
    {
        IsOvercharged.Value = true;
        NotifySetOvercharge(teamId, true);
    }

    [ServerRpc]
    private void RequestOverchargeUse(int teamId)
    {
        IsOvercharged.Value = false;
        NotifySetOvercharge(teamId, false);
    }

    [ObserversRpc]
    private void NotifySetOvercharge(int teamId, bool value)
    {
        OnOvercharged?.Invoke(teamId, value);
    }
}
