using System;
using UnityEngine;

public class OverchargedStatus : MonoBehaviour
{
    public static event Action<int, bool> OnOvercharged;
    public event Action<int> OnOverchargeRequest;
    public static event Action<int> OnUseOverchargeRequest;

    public bool IsOvercharged { get; set; }

    private void OnEnable()
    {
        Subscribe(); 
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    {
        OnOverchargeRequest += HandleLocalOverchargeRequest;
        OnUseOverchargeRequest += HandleLocalUseOverchargeRequest;
    }

    public void Unsubscribe()
    {
        OnOverchargeRequest -= HandleLocalOverchargeRequest;
        OnUseOverchargeRequest -= HandleLocalUseOverchargeRequest;
    }

    public void RequestOvercharge()
    {
        OnOverchargeRequest?.Invoke(TeamMember.localTeamId);
    }

    public static void RequestUseOvercharge(int teamId)
    {
        OnUseOverchargeRequest?.Invoke(teamId);
    }

    private void HandleLocalOverchargeRequest(int teamId)
    {
        ApplyOvercharge(teamId, true);
    }

    private static void HandleLocalUseOverchargeRequest(int teamId)
    {
        ApplyOvercharge(teamId, false);
    }

    public static void ApplyOvercharge(int teamId, bool value)
    {
        OnOvercharged?.Invoke(teamId, value);
    }
}
