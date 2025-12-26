using UnityEngine;
using FishNet.Object;

[RequireComponent(typeof(OverchargedStatus))]
public class OverchargedStatusNetworking : NetworkSingleton<OverchargedStatusNetworking>
{
    private OverchargedStatus overchargedStatus;

    public override void OnStartClient()
    {
        base.OnStartClient();
        overchargedStatus = GetComponent<OverchargedStatus>();
        overchargedStatus.Unsubscribe();

        overchargedStatus.OnOverchargeRequest += RequestOvercharge;
        OverchargedStatus.OnUseOverchargeRequest += HandleUseOverchargeRequest;
    }

    public void RequestOvercharge(int teamId)
    {
        SetOverchargeServerRpc(teamId);
    }

    private static void HandleUseOverchargeRequest(int teamId)
    {
        OverchargedStatusNetworking.Instance.Server_RequestUseOvercharge(teamId);
    }

    [ServerRpc]
    private void SetOverchargeServerRpc(int teamId)
    {
        NotifySetOvercharge(teamId, true);
    }

    [ServerRpc]
    private void Server_RequestUseOvercharge(int teamId)
    {
        NotifySetOvercharge(teamId, false);
    }

    [ObserversRpc]
    private void NotifySetOvercharge(int teamId, bool value)
    {
        OverchargedStatus.ApplyOvercharge(teamId, value);
        if (overchargedStatus.teamId == teamId)
        { 
            overchargedStatus.IsOvercharged = value;
        }
    }
}
