using System;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class ChargeStatusNetworking : NetworkBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Make sure the weapons and the")] private BatteryUseField[] batteryUseFields;

    public readonly SyncVar<bool[]> ChargeFieldStatus = new SyncVar<bool[]>();

    void OnEnable()
    {
        Array.ForEach(batteryUseFields, b => b.OnUpdateIsReady += RequestUpdateReady);
        ChargeFieldStatus.OnChange += OnChangeDebug;
    }
    
    void OnDisable()
    {
        Array.ForEach(batteryUseFields, b => b.OnUpdateIsReady -= RequestUpdateReady);
        ChargeFieldStatus.OnChange -= OnChangeDebug;
    }
    private void OnChangeDebug(bool[] prev, bool[] next, bool asServer)
    {
        Debug.Log("Update Network Value");
        Array.ForEach(next, n => Debug.Log(n ? "True" : "False"));
    }
    private void RequestUpdateReady(bool isReady = false) => UpdateIsReady(Owner);

    [TargetRpc(RunLocally = true)]
    private void UpdateIsReady(NetworkConnection target)
    {
        Debug.Log("Update Is Ready");
        bool[] newChargefieldStatus = new bool[batteryUseFields.Length];
        for (int i = 0; i < batteryUseFields.Length; i++)
        {
            newChargefieldStatus[i] = batteryUseFields[i].IsReady;
        }
        Array.ForEach(newChargefieldStatus, n => Debug.Log(n ? "True" : "False"));
        RequestValueChange(newChargefieldStatus);
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestValueChange(bool[] value)
    {
        ChargeFieldStatus.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDischarge()
    {
        TargetDischarge(Owner);
    }

    [TargetRpc]
    public void TargetDischarge(NetworkConnection target)
    {
        Debug.Log("Target Discharge");
        Array.ForEach(batteryUseFields, b => b.UseUpBatteries());
    }
}