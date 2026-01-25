using System;
using UnityEngine;

public class UseUpbatteriesDebug : MonoBehaviour {
    [SerializeField] private BatteryUseField[] batteryUseFields;

    [ContextMenu("Trigger")]
    public void UseUpBatteries() => Array.ForEach(batteryUseFields, b => b.UseUpBatteries());
}