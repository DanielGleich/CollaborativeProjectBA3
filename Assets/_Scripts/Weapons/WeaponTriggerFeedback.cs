using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WeaponTriggerFeedback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int weaponId = -1;
    [SerializeField] float duration = 5f;

    [Header("Events")]
    public UnityEvent OnFeedbackStart;
    public UnityEvent OnFeedbackStop;

    private void OnEnable()
    {
        WeaponTrigger.OnWeaponTriggered += WeaponTriggerNetworking_OnWeaponTriggered;
    }

    private void OnDisable()
    {
        WeaponTrigger.OnWeaponTriggered -= WeaponTriggerNetworking_OnWeaponTriggered;
    }

    private void WeaponTriggerNetworking_OnWeaponTriggered(int teamId, int triggeredWeaponId)
    {
        if (teamId == TeamMember.localTeamId && weaponId == triggeredWeaponId)
        {
            OnFeedbackStart?.Invoke();
            StartCoroutine(FeedbackProcedureWait());
        }
    }

    IEnumerator FeedbackProcedureWait()
    { 
        yield return new WaitForSeconds(duration);
        OnFeedbackStop?.Invoke();
    }
}
