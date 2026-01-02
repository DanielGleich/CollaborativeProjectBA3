using System;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    public static event Action<int> OnWeaponTrigger;

    public static void TriggerChargedWeapons(int teamId)
    {
        OverchargedStatus.RequestUseOvercharge(teamId);
        OnWeaponTrigger?.Invoke(teamId);
    }
}
