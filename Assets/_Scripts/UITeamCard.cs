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

    public Team currentTeam;

    public event Action<int, TeamRole, ulong> OnRequestProfile;

    private void OnEnable()
    {
        Team.OnTeamUpdate += UpdateTeamSlotProfiles;
    }

    private void OnDisable()
    {
        Team.OnTeamUpdate -= UpdateTeamSlotProfiles;
    }

    public void Init()
    {
        scientistProfile.CurrentSteamId = CSteamID.Nil;
        ratProfile.CurrentSteamId = CSteamID.Nil;
        teamTitle.text = "Team " + (currentTeam.id + 1);
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
        scientistProfile.CurrentSteamId = currentTeam.scientistPlayer;
        ratProfile.CurrentSteamId = currentTeam.ratPlayer;
    }
}
