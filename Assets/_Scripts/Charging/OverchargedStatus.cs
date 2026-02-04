using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;

public class OverchargedStatus : NetworkBehaviour
{
    public static event Action<int> OnUseOverchargeRequest;
    public static event Action<int, bool> OnOvercharged;

    public bool IsOvercharged = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
            OnUseOverchargeRequest += RequestOverchargeUse;
    }

    public void RequestOvercharge()
    {
        SetOverchargeServerRpc(TeamMember.localTeamId);
        OnOvercharged.Invoke(TeamMember.localTeamId, true);
    }
    public static void RequestUseOvercharge(int teamId)
    {
        OnUseOverchargeRequest?.Invoke(teamId);
    }

    [ServerRpc]
    private void SetOverchargeServerRpc(int teamId)
    {
        IsOvercharged = true;
        NotifySetOvercharge(teamId, true);
    }

    [ServerRpc]
    private void RequestOverchargeUse(int teamId)
    {
        IsOvercharged = false;
        NotifySetOvercharge(teamId, false);
    }

    [ObserversRpc]
    private void NotifySetOvercharge(int teamId, bool value)
    {
        OnOvercharged?.Invoke(teamId, value);
    }
}
