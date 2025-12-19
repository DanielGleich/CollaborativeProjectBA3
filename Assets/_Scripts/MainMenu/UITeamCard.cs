using Steamworks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeamCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UISteamProfile scientistProfile;
    [SerializeField] UISteamProfile ratProfile;
    [SerializeField] TextMeshProUGUI teamTitle;
    [SerializeField] Button JoinRatButton;
    [SerializeField] Button JoinScientistButton;

    public Team teamTemplate;
    public int currentTeamId;

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

        JoinRatButton.onClick.AddListener(RequestRatTeamSlot);
        JoinScientistButton.onClick.AddListener(RequestScientistTeamSlot);
    }

    void UpdateTeamSlotProfiles()
    {
        if (TeamManager.Instance == null || TeamManager.Instance.allTeams.TryGetValue(currentTeamId, out Team t) == false) return;
        scientistProfile.CurrentSteamId = t.scientistPlayer;
        ratProfile.CurrentSteamId = t.ratPlayer;
    }

    private void RequestRatTeamSlot()
    {
        if (TeamManager.Instance != null)
        {
            TeamManager.Instance.RequestSlot(currentTeamId, TeamRole.RAT, SteamUser.GetSteamID().m_SteamID);
        }
    }

    private void RequestScientistTeamSlot()
    { 
        if (TeamManager.Instance != null)
        {
            TeamManager.Instance.RequestSlot(currentTeamId, TeamRole.SCIENTIST, SteamUser.GetSteamID().m_SteamID);
        }    
    }
}
