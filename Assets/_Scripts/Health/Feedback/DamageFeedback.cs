using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;

public class DamageFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HealthNetworking health;

    [Header("Settings")]
    [SerializeField] float requiredDamageToTrigger = .1f;

    [Header("Events")]
    public UnityEvent<float> OnDamageTriggered = new UnityEvent<float>();

    private void OnEnable()
    {
        if(health == null)
            TeamManager.OnTeamReady += GetVehicleHealth;
        else 
            health.CurrentHealth.OnChange += CurrentHealth_OnChange;
    }

    private void OnDisable()
    {
        TeamManager.OnTeamReady -= GetVehicleHealth;

        if (health != null)
            health.CurrentHealth.OnChange -= CurrentHealth_OnChange;
    }

    private void GetVehicleHealth(Team team)
    {
        if (team.id != TeamMember.localTeamId) return;

        NetworkObject scientist = TeamManager.Instance.GetTeamMember(TeamMember.localTeamId, TeamRole.SCIENTIST);
        if (scientist == null)
        {
            Debug.LogWarning($"DamageFeedback {gameObject.name} did not find HealthNetworking-Component");
            return;
        }
        health = scientist.transform.GetComponentInChildren<HealthNetworking>();
        health.CurrentHealth.OnChange += CurrentHealth_OnChange;
    }

    private void CurrentHealth_OnChange(float prev, float next, bool asServer)
    {
        float damageTaken = (prev - next);
            Debug.Log(damageTaken);
        if (damageTaken > requiredDamageToTrigger)
        {
            OnDamageTriggered.Invoke(damageTaken);
        }
    }
}
