using UnityEngine;

public class HazardTrigger : Weapon
{
    protected override void Activate() => TriggerHazards();

    void TriggerHazards()
    {
        if (StageHazardManager.Instance != null)
            StageHazardManager.Instance.RequestTrigger();
        else 
            StageHazard.ActivateAllHazards();
    }
}
