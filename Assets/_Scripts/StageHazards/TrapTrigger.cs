using UnityEngine;
using UnityEngine.Events;

public class TrapTrigger : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Tooltip("Has to be set bevore play mode is entered")] private bool triggerOnStageHazardTriggered;
    [SerializeField] private UnityEvent onTriggered;
    [SerializeField, Min(0)] private float triggerCooldown = 2f;

    private float lastTriggerTimeStamp;

    void Awake()
    {
        lastTriggerTimeStamp = -triggerCooldown;
    }

    void OnEnable()
    {
        if(triggerOnStageHazardTriggered)
            StageHazard.OnActivateAll += Trigger;
    }
    void OnDisable()
    {
        if(triggerOnStageHazardTriggered)
            StageHazard.OnActivateAll -= Trigger;
    }
    [ContextMenu("Trigger Weapon")]
    public void Trigger()
    {
        if(Time.time - triggerCooldown < lastTriggerTimeStamp)
        {
            Debug.LogWarning("Weapon can't be triggerd because the cooldown is still active");
            return;
        }
        onTriggered?.Invoke();
        lastTriggerTimeStamp = Time.time;
    }
}