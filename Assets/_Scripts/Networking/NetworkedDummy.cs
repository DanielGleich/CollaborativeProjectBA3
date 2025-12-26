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
            GetNetworkedComponent(TeamManager.Instance.GetOtherTeamMemeber(teamMember.OwnerSteamId.Value).gameObject);
    }
    private void TeamIsReady(Team team)
    {
        if(team.id == teamMember.CurrentTeam.Value.id)
            GetNetworkedComponent(TeamManager.Instance.GetOtherTeamMemeber(teamMember.OwnerSteamId.Value).gameObject);
    }
    protected abstract void GetNetworkedComponent(GameObject other);
}
