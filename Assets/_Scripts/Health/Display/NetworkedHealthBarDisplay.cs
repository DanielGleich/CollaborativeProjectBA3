using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(CustomProgressBar))]
public class NetworkedHealthBarDisplay : NetworkedHealthDisplay
{
    CustomProgressBar bar;

    private void Awake()
    {
        bar = GetComponent<CustomProgressBar>();
    }

    private void OnEnable()
    {
        TeamManager.OnTeamReady += Init;
    }

    private void OnDisable()
    {
        TeamManager.OnTeamReady -= Init;
    }

    private void Init(Team t)
    {
        if (t.id != TeamMember.localTeamId && TeamMember.localTeamId == -1) return;
        NetworkObject player = TeamManager.Instance.GetTeamMember(TeamMember.localTeamId, TeamRole.SCIENTIST);
        if (player != null)
        {
            NetworkedHealth = player.transform.GetComponentInChildren<HealthNetworking>();
        }
    }

    protected override void UpdateHealth(float prev, float next, bool asServer)
    {
        bar.SetValue(next / NetworkedHealth.MaxHealth);
    }
}
