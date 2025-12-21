using System;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControlRoom : MonoBehaviour
{
    public int teamId = -1;
    public event Action OnForceAllWeaponsTrigger;

    public void RequestForceAllWeaponsTrigger()
    {
        OverchargedStatus.RequestUseOvercharge(teamId);
        OnForceAllWeaponsTrigger?.Invoke();
    }
}
