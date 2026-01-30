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
        TeamManager.OnAllTeamsReady += FindTeam;
        OverchargedStatus.OnOvercharged += OnTeamOvercharged;
    }
    private void OnDisable()
    {
        TeamManager.OnAllTeamsReady -= FindTeam;
        OverchargedStatus.OnOvercharged -= OnTeamOvercharged;
    }

    private void FindTeam()
    {
        if (gameObject.transform.root.TryGetComponent<NetworkObject>(out NetworkObject playerObject))
            ownerTeamId = TeamManager.Instance.GetTeamId(playerObject);

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
