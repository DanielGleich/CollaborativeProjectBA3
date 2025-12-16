using Steamworks;
using System;
using TMPro;
using UnityEngine;

public class UITeamCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UISteamProfile scientistProfile;
    [SerializeField] UISteamProfile ratProfile;
    [SerializeField] TextMeshProUGUI teamTitle;

    public Team teamTemplate;
    public int currentTeamId;

    public event Action<int, TeamRole, ulong> OnRequestProfile;

    private void OnEnable()
    {
        TeamManager.OnTeamUpdate += UpdateTeamSlotProfiles;
        UpdateTeamSlotProfiles();
    }

    private void OnDisable()
    {
        TeamManager.OnTeamUpdate -= UpdateTeamSlotProfiles;
    }

    public void SetCurrentTeam(Team team)
    {
        teamTemplate = team;
        currentTeamId = team.id;
        teamTitle.text = "Team " + (team.id + 1);
        UpdateTeamSlotProfiles();
    }

    public void RequestRatSlotClick()
    {
        OnRequestProfile?.Invoke(currentTeamId, TeamRole.RAT, SteamUser.GetSteamID().m_SteamID);
    }

    public void RequestScientistSlotClick()
    { 
        OnRequestProfile?.Invoke(currentTeamId, TeamRole.SCIENTIST, SteamUser.GetSteamID().m_SteamID);
    }

    void UpdateTeamSlotProfiles()
    {
        if (TeamManager.Instance == null || TeamManager.Instance.allTeams.TryGetValue(currentTeamId, out Team t) == false) return;
        scientistProfile.CurrentSteamId = t.scientistPlayer;
        ratProfile.CurrentSteamId = t.ratPlayer;
    }
}
