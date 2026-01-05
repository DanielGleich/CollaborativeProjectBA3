using UnityEngine;

public class TriggerWeaponInteraction : Interactable
{
    public override void Interact()
    {
        WeaponManager.TriggerChargedWeapons(TeamMember.localTeamId);
    }
}
