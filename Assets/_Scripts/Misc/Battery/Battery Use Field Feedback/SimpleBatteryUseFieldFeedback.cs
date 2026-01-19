using UnityEngine;

public class SimpleBatteryUseFieldFeedback : BatteryUseFieldFeedback
{
    [SerializeField] private Light l;
    protected override void UpdateIsReady(bool isReady)
    {
        l.enabled = isReady;
    }
}