using System;
using UnityEngine;

public class UseUpbatteriesDebug : MonoBehaviour {
    [SerializeField] private BatteryUseField[] batteryUseFields;

    [ContextMenu("Trigger")]
    private void UseUpBatteries() => Array.ForEach(batteryUseFields, b => b.UseUpBatteries());
}