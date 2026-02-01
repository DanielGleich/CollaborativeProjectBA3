using UnityEngine;
using UnityEngine.Events;

public class SimpleBatteryUseFieldFeedback : BatteryUseFieldFeedback
{
    [SerializeField] private UnityEvent<bool> onIsReady;
    [SerializeField] private UnityEvent onTriggerField;
    [SerializeField] private UnityEvent<int> onUpdateChargedBatteriesCount;

    protected override void OnEnable()
    {
        base.OnEnable();
        batteryUseField.OnDischargeBatteries += TriggerField;
        batteryUseField.OnUpdateBatteries += UpdateChargedBatteries;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        batteryUseField.OnDischargeBatteries -= TriggerField;
        batteryUseField.OnUpdateBatteries -= UpdateChargedBatteries;
    }
    protected override void UpdateIsReady(bool isReady) => onIsReady?.Invoke(isReady);
    private void TriggerField() => onTriggerField?.Invoke();
    private void UpdateChargedBatteries() => onUpdateChargedBatteriesCount?.Invoke(batteryUseField.ChargedBatteriesCount);
}