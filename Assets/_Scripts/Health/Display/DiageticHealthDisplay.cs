using System.Linq;
using GameKit.Dependencies.Utilities;
using UnityEngine;

/// <summary>
/// Shows the health of one battlbot with a specific team id by activating/ deactivating Lamps
/// </summary>
public class DiageticHealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Make sure they are in correct order")] private LampTweenAnimator[] lampAnimators;

    [Header("Settings")]
    [SerializeField, Min(0)] private int teamId;
    [SerializeField, Tooltip("false = round to int (when rounding instead of ceiling the display will show 0 health even bevore a player is dead)")] private bool ceilValues = true;

    [Header("Debug")]
    [SerializeField] private float debugHealth = 100;
    [SerializeField] private float debugMaxHealth = 100;

    private HealthNetworking healthNetworking;

    void OnEnable()
    {
        TeamManager.OnTeamReady += TryAssignHealthComponent;
    }
    void OnDisable()
    {
        TeamManager.OnTeamReady -= TryAssignHealthComponent;
        if (healthNetworking)
            healthNetworking.CurrentHealth.OnChange -= UpdateDisplay;
    }

    private void TryAssignHealthComponent(Team team)
    {
        if (teamId != team.id || healthNetworking != null)
            return;
        var scientistNetworkObject = TeamManager.Instance.GetTeamMember(teamId, TeamRole.SCIENTIST);
        if (!scientistNetworkObject)
            scientistNetworkObject = FindObjectsByType<TeamMember>(FindObjectsSortMode.None).ToList().Find(x => x.CurrentTeam.Value.id == teamId && x.CurrentRole.Value == TeamRole.SCIENTIST);
        if (!scientistNetworkObject)
        {
            Debug.LogError("Couldn't find team member");
            return;
        }
        healthNetworking = scientistNetworkObject.gameObject.GetComponentInChildren<HealthNetworking>();
        if (!healthNetworking)
        {
            Debug.LogError("Team member found, but networked health component wasn't found");
            return;
        }
        SetUp();
    }
    private void SetUp()
    {
        healthNetworking.CurrentHealth.OnChange += UpdateDisplay;
        UpdateDisplay(healthNetworking.CurrentHealth.Value, healthNetworking.CurrentHealth.Value, false);
    }
    private void UpdateDisplay(float prev, float next, bool asServer)
    {
        int activeLamps = ceilValues ? Mathf.CeilToInt(next / healthNetworking.MaxHealth * lampAnimators.Length) :
            Mathf.RoundToInt(next / healthNetworking.MaxHealth * lampAnimators.Length);

        for (int i = 0; i < lampAnimators.Length; i++)
        {
            lampAnimators[i].Activate(i < activeLamps);
        }
    }

    [ContextMenu("Show Debug Values")]
    private void DebugDisplay()
    {
        int activeLamps = ceilValues ? Mathf.CeilToInt(debugHealth / debugMaxHealth * lampAnimators.Length) :
            Mathf.RoundToInt(debugHealth / debugMaxHealth * lampAnimators.Length);

        for (int i = 0; i < lampAnimators.Length; i++)
        {
            lampAnimators[i].Activate(i < activeLamps);
        }
    }
}