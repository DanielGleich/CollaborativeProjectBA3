using UnityEngine;

public abstract class BatteryUseFieldFeedback : MonoBehaviour {
    [SerializeField] protected BatteryUseField batteryUseField;

    protected virtual void OnValidate()
    {
        if(!batteryUseField)
            batteryUseField = GetComponentInChildren<BatteryUseField>();
    }
    protected virtual void OnEnable()
    {
        batteryUseField.OnUpdateIsReady += UpdateIsReady;
        UpdateIsReady(batteryUseField.IsReady);
    }
    protected virtual void OnDisable()
    {
        batteryUseField.OnUpdateIsReady += UpdateIsReady;
    }

    protected abstract void UpdateIsReady(bool isReady);
}