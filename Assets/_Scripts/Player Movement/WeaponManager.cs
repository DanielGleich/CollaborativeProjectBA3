using System;

public class WeaponManager : Singleton<WeaponManager>
{
    public static event Action<int> OnWeaponTrigger;

    public static void TriggerChargedWeapons(int teamId)
    {
        OnWeaponTrigger?.Invoke(teamId);
        OverchargedStatus.RequestUseOvercharge(teamId);
    }
}
