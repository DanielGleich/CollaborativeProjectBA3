using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct UICountdownElement
{
    public int countdownValue;
    public GameObject uiObject;
}

public class GameCountdownDisplay : MonoBehaviour
{
    [Header("Countdown Elements")]
    [SerializeField] List<UICountdownElement> countdownElements = new();

    [Header("Events")]
    [SerializeField] UnityEvent onCountdownEnd;

    private Dictionary<int, GameObject> countdownMap;
    private bool timerStarted;
    private int lastCountdown = int.MinValue;

    private void OnEnable()
    {
        BuildCountdownMap();
        GameManager.OnInitialized += GameManager_OnInitialized;
    }

    private void OnDisable()
    {
        GameManager.OnInitialized -= GameManager_OnInitialized;
        if (GameManager.Instance != null && GameManager.Instance.PreGameCountdown != null)
            GameManager.Instance.PreGameCountdown.OnChange -= PreGameCountdown_OnChange;
    }

    private void BuildCountdownMap()
    {
        countdownMap = new Dictionary<int, GameObject>(countdownElements.Count);
        foreach (var element in countdownElements)
        {
            if (element.uiObject != null)
            {
                countdownMap[element.countdownValue] = element.uiObject;
                element.uiObject.SetActive(false); // alle initial aus
            }
        }
    }

    private void GameManager_OnInitialized()
    {
        if (GameManager.Instance != null && GameManager.Instance.PreGameCountdown != null)
            GameManager.Instance.PreGameCountdown.OnChange += PreGameCountdown_OnChange;
    }

    private void PreGameCountdown_OnChange(FishNet.Object.Synchronizing.SyncTimerOperation op, float prev, float next, bool asServer)
    {
        switch (op)
        {
            case FishNet.Object.Synchronizing.SyncTimerOperation.Start:
                timerStarted = true;
                if (GameManager.Instance?.PreGameCountdown != null)
                    UpdateCountdownVisual(Mathf.CeilToInt(GameManager.Instance.PreGameCountdown.Remaining));
                break;

            case FishNet.Object.Synchronizing.SyncTimerOperation.Finished:
                HandleCountdownFinished();
                break;
        }
    }

    private void Update()
    {
        if (!timerStarted) return;

        var gm = GameManager.Instance;
        if (gm == null || gm.PreGameCountdown == null) return;

        int countdownValue = Mathf.CeilToInt(gm.PreGameCountdown.Remaining);
        if (countdownValue == lastCountdown) return;

        UpdateCountdownVisual(countdownValue);
    }

    private void UpdateCountdownVisual(int value)
    {
        if (countdownMap != null && countdownMap.TryGetValue(lastCountdown, out var lastGo) && lastGo != null)
            lastGo.SetActive(false);

        if (countdownMap != null && countdownMap.TryGetValue(value, out var go) && go != null)
            go.SetActive(true);

        lastCountdown = value;
    }

    private void HandleCountdownFinished()
    {
        timerStarted = false;

        if (countdownMap != null)
        {
            foreach (var kvp in countdownMap)
            {
                if (kvp.Value != null)
                    kvp.Value.SetActive(false);
            }
        }
        onCountdownEnd?.Invoke();
        gameObject.SetActive(false);
    }
}
