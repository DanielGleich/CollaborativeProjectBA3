using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ChargeStatus))]
public class WeaponTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Weapon targetWeapon;

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
        WeaponManager.OnWeaponTrigger += TriggerChargedWeapons;
    }

    public void Unsubscribe()
    {
        OnTriggerRequest -= TriggerWeapon;
        WeaponManager.OnWeaponTrigger -= TriggerChargedWeapons;
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

    public void ForceTriggerWeapon(int teamId)
    {
        if (TeamMember.localTeamId != teamId) return;
        StopAllCoroutines();
        OnCooldownCancelled?.Invoke();
        targetWeapon.TryActivate();
        OnTriggered?.Invoke();
        StartCoroutine(Cooldown());
        Debug.Log(targetWeapon.name + " successfully force-triggered");
    }

    public void ForceTriggerWeapon()
    {
        ForceTriggerWeapon(TeamMember.localTeamId);
    }

    public void ForceTriggerWeaponAnimation()
    {
        Debug.Log("Method not Implemented yet");
    }

    private void TriggerChargedWeapons(int teamId)
    {
        Debug.Log($"{gameObject.name} - Team {teamId} triggered");
        if (TeamMember.localTeamId != teamId) return;

        if (chargeStatus.IsCharged)
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
