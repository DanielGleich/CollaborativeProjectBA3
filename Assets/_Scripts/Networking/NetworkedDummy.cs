using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for forwarding values ​​from network components from other team members and keeping a placeholder value (if necessary).
/// </summary>
public abstract class NetworkedDummy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TeamMember teamMember;

    void OnValidate()
    {
        if(!teamMember)
            teamMember = GetComponentInParent<TeamMember>();
    }
    void OnEnable()
    {
        teamMember.CurrentTeam.OnChange += TeamAssigned;
        TeamManager.OnTeamReady += TeamIsReady;
    }
    void OnDisable()
    {
        teamMember.CurrentTeam.OnChange -= TeamAssigned;
        TeamManager.OnTeamReady -= TeamIsReady;
    }
    private void TeamAssigned(Team prev, Team next, bool asServer)
    {
        if(TeamManager.Instance.IsTeamReady(next))
            TryGetNetworkedComponent();
    }
    private void TeamIsReady(Team team)
    {
        if(team.id == teamMember.CurrentTeam.Value.id)
            TryGetNetworkedComponent();
    }
    private void TryGetNetworkedComponent()
    {
        Debug.Log($"Owner SteamId {teamMember.OwnerSteamId.Value}");
        var otherTeamMember = TeamManager.Instance.GetOtherTeamMember(teamMember.OwnerSteamId.Value);
        Debug.Log($"Other Team Member Found: {otherTeamMember != null}");
        // Different Method for finding the other team member in case getting it trough the team manager didn't work
        if(!otherTeamMember)
        {
            otherTeamMember = FindObjectsByType<TeamMember>(FindObjectsSortMode.None).ToList().Find(x => x.CurrentTeam.Value.id == teamMember.CurrentTeam.Value.id && x.CurrentRole.Value != teamMember.CurrentRole.Value);
            Debug.Log($"Other Team Member Found: {otherTeamMember != null}");
        }
        if(otherTeamMember)
            GetNetworkedComponent(otherTeamMember.gameObject);
        else 
            Debug.LogError("Other team member not found");
    }
    protected abstract void GetNetworkedComponent(GameObject other);
}
