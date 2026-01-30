using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WeaponTriggerFeedback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int weaponId = -1;
    [SerializeField] float duration = 5f;

    int ownerTeamId = -1;

    [Header("Events")]
    public UnityEvent OnFeedbackStart;
    public UnityEvent OnFeedbackStop;

    private void OnEnable()
    {
        TeamManager.OnAllTeamsReady += FindTeam;
        WeaponTrigger.OnWeaponTriggered += OnTeamWeaponTrigger;
    }

    private void OnDisable()
    {
        TeamManager.OnAllTeamsReady += FindTeam;
        WeaponTrigger.OnWeaponTriggered -= OnTeamWeaponTrigger;
    }

    private void FindTeam()
    {
        if (gameObject.transform.root.TryGetComponent<NetworkObject>(out NetworkObject playerObject))
            ownerTeamId = TeamManager.Instance.GetTeamId(playerObject);

        if (ownerTeamId == -1)
            gameObject.SetActive(false);
    }

    private void OnTeamWeaponTrigger(int teamId, int triggeredWeaponId)
    {
        if (teamId == ownerTeamId && weaponId == triggeredWeaponId)
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
