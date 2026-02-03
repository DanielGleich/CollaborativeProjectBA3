using UnityEngine;

public class BatterieUseFieldLampFeedback : BatteryUseFieldFeedback
{
    [Header("References")]
    [SerializeField] private Lamp[] lamps;

    protected override void OnEnable()
    {
        base.OnEnable();
        batteryUseField.OnUpdateBatteries += UpdateLamps;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        batteryUseField.OnUpdateBatteries -= UpdateLamps;
    }

    private void UpdateLamps()
    {
        int chargedBatteriesCount = batteryUseField.ChargedBatteriesCount;
        //Debug.Log($"Active Lamps: {chargedBatteriesCount}");
        for(int i = 0; i < lamps.Length; i ++)
        {
            lamps[i].Activate(i < chargedBatteriesCount);
        }
    }

    protected override void UpdateIsReady(bool isReady) => UpdateLamps();
}