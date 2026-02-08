using System;
using UnityEngine;

public class ChargeStatusDummy : NetworkedDummy
{
    private ChargeStatusNetworking chargeStatusNetworking;
    public bool[] ChargeStatus => chargeStatusNetworking ? chargeStatusNetworking.ChargeFieldStatus.Value : null;
    public event Action<bool[]> OnUpdateChargeStatus;
    protected override void GetNetworkedComponent(GameObject other)
    {
        chargeStatusNetworking = other.GetComponent<ChargeStatusNetworking>();
        if (chargeStatusNetworking)
        {
            chargeStatusNetworking.ChargeFieldStatus.OnChange += UpdateChargeStatus;
        }
        else
            Debug.LogError("Component not found");
    }
    void OnDestroy()
    {
        if (chargeStatusNetworking)
            chargeStatusNetworking.ChargeFieldStatus.OnChange -= UpdateChargeStatus;
    }
    public void NotifyWeaponTriggering()
    {
        if(chargeStatusNetworking)
            chargeStatusNetworking.RequestDischarge();
    }

    private void UpdateChargeStatus(bool[] prev, bool[] next, bool asServer)
    {
        OnUpdateChargeStatus?.Invoke(next);
    }
}