using System;
using UnityEngine;

/// <summary>
/// Keeps track of battery charge amount and charge completion and updates other components about it
/// </summary>
public class Battery : MonoBehaviour {

    [field: Header("Settings")]
    [field: SerializeField, Min(0)] public float MaxChargeVolume {get; private set;} = 1f;
    [SerializeField] public float fullyChargedMargin = .1f;

    private float chargeAmount;
    public event Action<float> OnUpdateChargeAmount;
    public float ChargeAmount
    {
        get => chargeAmount;
        set
        {
            value = Mathf.Clamp(value,0,MaxChargeVolume);
            if(chargeAmount == value)
                return;
            chargeAmount = value;
            OnUpdateChargeAmount?.Invoke(value);
            IsFullyCharged = value >= MaxChargeVolume - fullyChargedMargin;
        }
    }
    public event Action<bool> OnUpdateFullyCharged;
    private bool isFullyCharged;
    public bool IsFullyCharged
    {
        get => isFullyCharged;
        private set
        {
            if(isFullyCharged == value)
                return;
            isFullyCharged = value;
            OnUpdateFullyCharged?.Invoke(value);
        }
    }
}