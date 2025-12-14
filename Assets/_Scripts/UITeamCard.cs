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

    public Team currentTeam { get; private set; }

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
        currentTeam = team;
        teamTitle.text = "Team " + (currentTeam.id + 1);
        UpdateTeamSlotProfiles();
    }

    public void RequestRatSlotClick()
    {
        OnRequestProfile?.Invoke(currentTeam.id, TeamRole.RAT, SteamUser.GetSteamID().m_SteamID);
    }

    public void RequestScientistSlotClick()
    { 
        OnRequestProfile?.Invoke(currentTeam.id, TeamRole.SCIENTIST, SteamUser.GetSteamID().m_SteamID);
    }

    void UpdateTeamSlotProfiles()
    {
        if (currentTeam == null) return;
        scientistProfile.CurrentSteamId = currentTeam.scientistPlayer;
        ratProfile.CurrentSteamId = currentTeam.ratPlayer;
    }
}
