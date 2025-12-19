using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnEnable()
    {
        TeamManager.OnTeamReady += TeamManager_OnTeamReady;
        TeamManager.OnAllTeamsReady += TeamManager_OnAllTeamsReady;
    }

    private void OnDisable()
    {
        TeamManager.OnTeamReady -= TeamManager_OnTeamReady;
        TeamManager.OnAllTeamsReady -= TeamManager_OnAllTeamsReady;
    }

    private void TeamManager_OnTeamReady(Team obj)
    {
        Debug.Log($"ONTEAMREADY EVENT - TEAM {obj.id}");
    }
    private void TeamManager_OnAllTeamsReady()
    {
        Debug.Log($"ONALLTEAMREADY EVENT");
    }

    void Update()
    {
        Debug.Log($"Team 0 => {TeamManager.Instance?.IsTeamReady(0)}");
        Debug.Log($"Team 1 => {TeamManager.Instance?.IsTeamReady(1)}");
        Debug.Log($"AllTeamsReady => {TeamManager.Instance?.AllTeamsReady.Value}");
    }
}
