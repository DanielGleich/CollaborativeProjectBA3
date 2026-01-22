using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatteryUseField : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int requiredCharedBatteries = 2;
    [SerializeField] private List<Battery> batteries = new();

    private bool isReady;
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
        int chargedBatteries = batteries.Count(b => b.IsFullyCharged);
        IsReady = requiredCharedBatteries <= chargedBatteries;
    }
    public void UseUpBatteries()
    {
        if (!IsReady)
            return;
        for (int i = 0; i < requiredCharedBatteries; i++)
        {
            batteries[i].ChargeAmount = 0;
        }
        CheckBatteryCharging();
    }
}