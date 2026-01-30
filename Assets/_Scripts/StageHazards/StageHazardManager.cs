using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameKit.Dependencies.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHazardManager : NetworkSingleton<StageHazardManager>
{
    protected override bool _perClient { get; } = false;

    [Header("Settings")]
    [field: SerializeField] public float InitialWaitTime { private set; get; } = 5;
    [field: SerializeField] public float TimeBetweenActivations { private set; get; } = 5;
    [field: SerializeField, Min(0)] public Vector2 Duration { private set; get; } = new Vector2(5,5);
    [field: SerializeField, Min(0)] public Vector2 TriggerOffset { private set; get; } = new Vector2(0,2);

    public readonly SyncList<GameObject> allStageHazards = new SyncList<GameObject>();


    [Server, ContextMenu("Start")]
    public void InitializeManager()
    {
        if (allStageHazards.Count == 0)
        {
            StageHazard[] hazards = FindObjectsByType<StageHazard>(FindObjectsSortMode.None);
            foreach (var hazard in hazards)
            {
                allStageHazards.Add(hazard.gameObject);
            }
        }
        foreach (var hazard in allStageHazards)
            StartCoroutine(Trigger(hazard, 1));

        StartCoroutine(WaitPhase(InitialWaitTime));
    }

    [Server]
    IEnumerator WaitPhase(float time)
    {
        yield return new WaitForSeconds(time);
        if (allStageHazards.Count > 0)
        {
            if (TriggerOffset != Vector2.zero)
            {
                List<int> allIds = new List<int>();
                for (int i = 0; i < allStageHazards.Count; i++)
                    allIds.Add(i);
                allIds.Shuffle();

                for(int i = 0; i < allIds.Count; i++)
                    StartCoroutine(DelayedTrigger(UnityEngine.Random.Range(TriggerOffset.x, TriggerOffset.y), allStageHazards[i]));
            }
            else
            {
                foreach (var hazard in allStageHazards)
                    StartCoroutine(Trigger(hazard));
            }
        }
        StartCoroutine(WaitPhase(TimeBetweenActivations));
    }

    IEnumerator Trigger(GameObject stageHazard, float overwriteDuration = 0)
    {
        NotifyHazardActivate(stageHazard);
        float time = (Duration.x == Duration.y) ? Duration.x : UnityEngine.Random.Range(Duration.x,Duration.y);

        if (overwriteDuration > 0)
            time = overwriteDuration;

        if (time <= 0) yield break;

        yield return new WaitForSeconds(time);
        NotifyHazardDeactivate(stageHazard);
    }

    [Server]
    IEnumerator DelayedTrigger(float delay, GameObject stageHazard)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Trigger(stageHazard));
    }

    [ObserversRpc]
    private void NotifyHazardActivate(GameObject hazard)
    {
        hazard.GetComponent<StageHazard>().Activate();
    }

    [ObserversRpc]
    private void NotifyHazardDeactivate(GameObject hazard)
    {
        hazard.GetComponent<StageHazard>().Deactivate();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestTrigger()
    {
        HandleNetworkedTrigger();
    }

    [ObserversRpc]
    private void HandleNetworkedTrigger()
    {
        StageHazard.ActivateAllHazards();
    }
}
