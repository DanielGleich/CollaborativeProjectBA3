using System;
using UnityEngine;

public class StageHazard : MonoBehaviour
{
    public static event Action OnTriggered;

    public static void TriggerAllHazards()
    {
        OnTriggered?.Invoke();
    }

    [ContextMenu("Debug Trigger")]
    private void DebugTrigger()
    {
        TriggerAllHazards();
    }
}
