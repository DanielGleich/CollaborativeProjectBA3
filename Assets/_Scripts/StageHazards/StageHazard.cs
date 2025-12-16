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
    public void DebugTrigger()
    {
        TriggerAllHazards();
    }
}
