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
        TeamManager.OnTeamReady += FindTeam;
        WeaponTrigger.OnWeaponTriggered += OnTeamWeaponTrigger;
    }

    private void OnDisable()
    {
        TeamManager.OnTeamReady += FindTeam;
        WeaponTrigger.OnWeaponTriggered -= OnTeamWeaponTrigger;
    }

    private void FindTeam(Team team)
    {
        if (ownerTeamId != -1) return;
        if (gameObject.transform.root.TryGetComponent<NetworkObject>(out NetworkObject playerObject))
        {
            int ownerId = playerObject.OwnerId;
            if (team.scientistPlayerClientId == ownerId || team.ratPlayerClientId == ownerId)
            {
                ownerTeamId = team.id;
            }
        }

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
