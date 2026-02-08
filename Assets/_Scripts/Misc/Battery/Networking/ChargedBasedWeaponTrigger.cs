using System;
using UnityEngine;

public class ChargedBasedWeaponTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private ChargeStatusDummy chargeStatusDummy;

    [Header("Settings")]
    [SerializeField, Min(0)] private float coolDown = .25f;
    [SerializeField] private bool godMode = false;

    private float lastTriggerTime;

    void Awake()
    {
        // Make sure the weapons can be fired instantly
        lastTriggerTime = -coolDown;
    }

    public void Activate()
    {
        if (godMode)
        {
            Array.ForEach(weapons, weapon => weapon.TryActivate());
            return;
        }
        if(Time.time < lastTriggerTime + coolDown)
        {
            Debug.LogWarning("Cooldown still active");
            return;
        }
        if (chargeStatusDummy.ChargeStatus == null)
            return;
        for (int i = 0; i < chargeStatusDummy.ChargeStatus.Length; i++)
        {
            if (chargeStatusDummy.ChargeStatus[i])
                weapons[i].TryActivate();
        }
        chargeStatusDummy.NotifyWeaponTriggering();
        lastTriggerTime = Time.time;
    }
}