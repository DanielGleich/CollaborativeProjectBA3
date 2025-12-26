using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ChargeStatus))]
public class WeaponTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Weapon targetWeapon;
    [field: SerializeField] public VehicleControlRoom ControlRoom { private set; get; }

    [Header("Settings")]
    [field: SerializeField] public float WeaponCooldown { private set; get; } = 0;
    public bool IsCooldown { get; private set; } = false;

    private ChargeStatus chargeStatus;
    public event Action OnTriggered;
    public event Action OnTriggerRequest;
    public event Action OnCooldownStart;
    public event Action OnCooldownCancelled;
    public event Action OnCooldownFinished;

    private void Awake()
    {
        chargeStatus = GetComponent<ChargeStatus>();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    {
        OnTriggerRequest += TriggerWeapon;
        ControlRoom.OnForceAllWeaponsTrigger += ForceTriggerWeapon;
    }

    public void Unsubscribe()
    { 
        OnTriggerRequest -= TriggerWeapon;
        ControlRoom.OnForceAllWeaponsTrigger -= ForceTriggerWeapon;
    }

    private void TriggerWeapon()
    {
        if (IsCooldown == false)
        {
            targetWeapon.TryActivate();
            OnTriggered?.Invoke();
            StartCoroutine(Cooldown());
            Debug.Log(targetWeapon.name + " successfully triggered");
        }
    }

    public void ForceTriggerWeapon()
    {
        StopAllCoroutines();
        OnCooldownCancelled?.Invoke();
        targetWeapon.TryActivate();
        OnTriggered?.Invoke();
        StartCoroutine(Cooldown());
        Debug.Log(targetWeapon.name + " successfully force-triggered");
    }

    public void TryTriggerWeapon()
    {
        if (chargeStatus.IsOvercharged)
        {
            ControlRoom.RequestForceAllWeaponsTrigger();
        }
        else if (chargeStatus.IsCharged)
        {
            OnTriggerRequest?.Invoke();
        }
    }

    IEnumerator Cooldown()
    {
        if (WeaponCooldown <= 0) yield break;
        IsCooldown = true;
        OnCooldownStart?.Invoke();
        yield return new WaitForSeconds(WeaponCooldown);
        IsCooldown = false;
        OnCooldownFinished?.Invoke();
    }
}
