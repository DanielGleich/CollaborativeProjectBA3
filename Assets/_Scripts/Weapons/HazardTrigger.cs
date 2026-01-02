using UnityEngine;

public class HazardTrigger : Weapon
{
    protected override void Activate() => TriggerHazards();

    void TriggerHazards()
    {
        StageHazardManager.Instance.RequestTrigger();
    }
}
