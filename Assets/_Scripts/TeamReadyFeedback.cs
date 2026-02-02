using UnityEngine;
using UnityEngine.Events;

public class TeamReadyFeedback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int teamId = -1;
    [SerializeField] bool TriggerIfTeamLocalOnly = true;

    public UnityEvent OnTeamReady = new UnityEvent();

    private void OnEnable()
    {
        TeamManager.OnTeamReady += OnTrigger;
    }

    private void OnDisable()
    {
        TeamManager.OnTeamReady -= OnTrigger;
    }

    private void OnTrigger(Team teamReady)
    {
        if (teamId == teamReady.id)
        {
            if (TriggerIfTeamLocalOnly == false || (TriggerIfTeamLocalOnly == true && TeamMember.localTeamId == teamId))
                OnTeamReady?.Invoke();
        }
    }
}
