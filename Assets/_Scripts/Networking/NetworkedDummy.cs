using UnityEngine;

/// <summary>
/// Responsible for forwarding values ​​from network components from other team members and keeping a placeholder value (if necessary).
/// </summary>
public abstract class NetworkedDummy : MonoBehaviour
{
    // [Header("References")]
    // [SerializeField] private TeamMember teamMember;

    void OnValidate()
    {
        // if(!teamMember)
            // teamMember = GetComponentInParent<TeamMember>();
    }
    void OnEnable()
    {
        // teamMember.OnAssign += TeamAssigned();
        // TeamManager.OnTeamReady += TeamIsReady();
    }
    void OnDisable()
    {
        // teamMember.OnAssign -= TeamAssigned();
        // TeamManager.OnTeamReady -= TeamIsReady();
    }
    private void TeamAssigned()
    {
        // Check if team is complete => if it is trigger GetNetworkedComponent
    }
    private void TeamIsReady()
    {
        // Check if TeamMember is already assigned => if not return
        // Check if your Team is ready => if it is trigger GetNetworkedComponent
    }
    protected abstract void GetNetworkedComponent(GameObject other);
}
