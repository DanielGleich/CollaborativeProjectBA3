using Steamworks;
using System;
using UnityEngine;

public class UITeamCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UISteamProfile scientistProfile;
    [SerializeField] UISteamProfile ratProfile;

    public Team currentTeam;

    public event Action<int, TeamRole, ulong> OnRequestProfile;

    private void Start()
    {
        scientistProfile.CurrentSteamId = CSteamID.Nil;
        ratProfile.CurrentSteamId = CSteamID.Nil;
    }

    public void RequestRatSlotClick()
    {
        OnRequestProfile?.Invoke(currentTeam.id, TeamRole.RAT, SteamUser.GetSteamID().m_SteamID);
    }

    public void RequestScientistSlotClick()
    { 
        OnRequestProfile?.Invoke(currentTeam.id, TeamRole.SCIENTIST, SteamUser.GetSteamID().m_SteamID);
    }

    public bool IsSlotAvailable(TeamRole role)
    {
        switch (role) 
        {
            case TeamRole.SCIENTIST:
                return scientistProfile.CurrentSteamId == CSteamID.Nil;
            
            case TeamRole.RAT:
                return ratProfile.CurrentSteamId == CSteamID.Nil;
        }
        return true;
    }

    public void UpdateTeamSlotProfile(TeamRole role, CSteamID steamId)
    {
        switch (role)
        {
            case TeamRole.SCIENTIST:
                scientistProfile.CurrentSteamId = steamId;
                break;

            case TeamRole.RAT:
                ratProfile.CurrentSteamId = steamId;
                break;
        }
    }
}
