using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatteryUseField : MonoBehaviour
{
    [Header("Settings")]
    [field: SerializeField] public int RequiredChargedBatteries { get; private set; } = 2;
    [field: SerializeField] public List<Battery> batteries { get; private set; } = new();
    [SerializeField] private bool dischargeAll;

    public int ChargedBatteriesCount => batteries.Count(b => b.IsFullyCharged);

    private bool isReady;
    public event Action OnUpdateBatteries;
    public event Action<bool> OnUpdateIsReady;
    public bool IsReady
    {
        get => isReady;
        private set
        {
            if (value == isReady)
                return;
            isReady = value;
            OnUpdateIsReady?.Invoke(value);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Battery>(out var battery))
        {
            if (batteries.Contains(battery))
                return;
            batteries.Add(battery);
            CheckBatteryCharging();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Battery>(out var battery))
        {
            if (!batteries.Contains(battery))
                return;
            batteries.Remove(battery);
            CheckBatteryCharging();
        }
    }
    private void CheckBatteryCharging()
    {
        OnUpdateBatteries?.Invoke();
        int chargedBatteries = batteries.Count(b => b.IsFullyCharged);
        IsReady = RequiredChargedBatteries <= chargedBatteries;
    }
    public void UseUpBatteries()
    {
        if (!IsReady)
            return;
        int charges = 0;
        foreach(Battery b in batteries)
        {
            if(b.IsFullyCharged || dischargeAll)
            {
                b.ChargeAmount = 0;
                charges++;
            }
            if(charges >= RequiredChargedBatteries && !dischargeAll)
                break;
        }
        CheckBatteryCharging();
    }
}