using UnityEngine;

public class BatterieUseFieldLampFeedback : BatteryUseFieldFeedback
{
    [Header("References")]
    [SerializeField] private Animator[] lampAnimators;

    [Header("Settings")]
    [SerializeField] private string parameterName = "Active";

    private int parameterHash;

    void Awake()
    {
        parameterHash = Animator.StringToHash(parameterName);
    }

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
        for(int i = 0; i < lampAnimators.Length; i ++)
        {
            lampAnimators[i].SetBool(parameterHash, i < chargedBatteriesCount);
        }
    }

    protected override void UpdateIsReady(bool isReady)
    {
        // Do Noting
    }
}