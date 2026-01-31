using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;

public class OverchargedFeedback : MonoBehaviour
{
    public UnityEvent OnOverchargeStart = new UnityEvent();
    public UnityEvent OnOverchargeFinish = new UnityEvent();

    int ownerTeamId = -1;

    private void OnEnable()
    {
        TeamManager.OnTeamReady += FindTeam;
        OverchargedStatus.OnOvercharged += OnTeamOvercharged;
    }
    private void OnDisable()
    {
        TeamManager.OnTeamReady -= FindTeam;
        OverchargedStatus.OnOvercharged -= OnTeamOvercharged;
    }

    private void FindTeam(Team team)
    {
        if (ownerTeamId != -1) return;
        if (gameObject.transform.root.TryGetComponent<NetworkObject>(out NetworkObject playerObject))
        { 
            int ownerId = playerObject.OwnerId;
            if ( team.scientistPlayerClientId == ownerId || team.ratPlayerClientId == ownerId)
            {
                ownerTeamId = team.id;
            }
        }

        if (ownerTeamId == -1)
            gameObject.SetActive(false);
    }

    private void OnTeamOvercharged(int chargedTeamId, bool isOvercharged)
    {
        if (ownerTeamId == chargedTeamId)
        {
            if (isOvercharged)
                OnOverchargeStart?.Invoke();
            else
                OnOverchargeFinish?.Invoke();
        }
    }
}
