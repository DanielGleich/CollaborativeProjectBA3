using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChargingPad : MonoBehaviour
{
    [Header("References")]
    [field: SerializeField] public ChargingPadTrigger Trigger { private set; get; }

    [Header("Options")]
    [field: SerializeField] public float Cooldown { private set; get; } = 0f;
    [field: SerializeField] public bool TriggerOnce { private set; get; } = false;

    public bool IsActive { private set; get; }
    public bool IsCooldown { private set; get; }

    public static event Action<ChargingPad> OnPadActivated;
    public static event Action<ChargingPad> OnPadDeactivated;

    public UnityEvent OnActivate = new UnityEvent();
    public UnityEvent OnDeactivate = new UnityEvent();
    public UnityEvent OnCooldownStart = new UnityEvent();
    public UnityEvent OnCooldownStop = new UnityEvent();

    private void OnEnable()
    {
        Subscribe();
        Deactivate();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    {
        Trigger.OnCharging += OverchargeVehicle;
    }

    public void Unsubscribe()
    {
        Trigger.OnCharging -= OverchargeVehicle;
    }

    public void Activate()
    {
        if (IsActive == true) return;
        IsActive = true;
        OnActivate?.Invoke();
        OnPadActivated?.Invoke(this);
    }

    public void Deactivate()
    {
        if (IsActive == false) return;
        IsActive = false;
        OnDeactivate?.Invoke();
        OnPadDeactivated?.Invoke(this);
    }

    public void StartCooldown()
    {
        StartCoroutine(CooldownProcess());
    }

        IEnumerator CooldownProcess()
    {
        if (Cooldown <= 0) yield break;

        IsCooldown = true;
        OnCooldownStart?.Invoke();
        
        yield return new WaitForSeconds(Cooldown);
        
        IsCooldown = false;
        if (IsActive)
            OnCooldownStop?.Invoke();
    }

    private void OverchargeVehicle(GameObject vehicle)
    {
        if (IsActive && IsCooldown == false)
        {
            OverchargedStatus[] chargingStatuses = vehicle.transform.root.GetComponentsInChildren<OverchargedStatus>();
            foreach (OverchargedStatus chargingStatus in chargingStatuses)
            {
                if (chargingStatus.IsOvercharged == false)
                {
                    chargingStatus.RequestOvercharge();
                    StartCooldown();
                    if (TriggerOnce)
                    {
                        Deactivate();
                        return;
                    }
                }
            }
        }
    }
}
