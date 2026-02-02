using UnityEngine;
using UnityEngine.Events;

public class AllTeamsReadyFeedback : MonoBehaviour
{
    public UnityEvent OnTeamsReady = new UnityEvent();

    private void OnEnable()
    {
        TeamManager.OnAllTeamsReady += OnTrigger;
    }

    private void OnDisable()
    {
        TeamManager.OnAllTeamsReady -= OnTrigger;
    }

    private void OnTrigger()
    {
        OnTeamsReady?.Invoke();
    }
}
