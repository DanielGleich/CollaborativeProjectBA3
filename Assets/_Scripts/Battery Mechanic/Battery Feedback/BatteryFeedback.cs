using UnityEngine;

public abstract class BatteryFeedback : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected Battery battery;

    void OnValidate()
    {
        if (!battery)
            battery = GetComponent<Battery>();
    }
    void OnEnable()
    {
        battery.OnUpdateChargeAmount += UpdateChargeAmountFeedback;
        UpdateChargeAmountFeedback(battery.ChargeAmount);
        battery.OnUpdateFullyCharged += UpdateChargeCompletionFeedback;
        UpdateChargeCompletionFeedback(battery.IsFullyCharged);
    }
    void OnDisable()
    {
        battery.OnUpdateChargeAmount -= UpdateChargeAmountFeedback;
        battery.OnUpdateFullyCharged -= UpdateChargeCompletionFeedback;
    }
    protected abstract void UpdateChargeCompletionFeedback(bool chargeAmpount);
    protected abstract void UpdateChargeAmountFeedback(float isCharged);                                                             
}