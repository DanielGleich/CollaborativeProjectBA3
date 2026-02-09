using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAfterDelay : MonoBehaviour
{
    [SerializeField] private UnityEvent onTimeUp;

    [Header("Settings")]
    [SerializeField, Min(0)] private float defaultDelay = .5f;
    [SerializeField] private bool startCoroutineOnEnable = true;

    void OnEnable()
    {
        if (startCoroutineOnEnable)
            StartCoroutine(TriggerAfterDelayRoutine());
    }

    public IEnumerator TriggerAfterDelayRoutine(float delay = -1)
    {
        if (delay < 0)
            delay = defaultDelay;
        yield return new WaitForSeconds(delay);
        onTimeUp?.Invoke();
    }
}