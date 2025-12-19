using FishNet;
using FishNet.Object;
using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] private Button startLobbyButton;
    [SerializeField] private GameObject joinContainer, lobbyContainer;
    [SerializeField] private TMP_Text title, id;
    [SerializeField] private List<UISteamProfile> lobbyIcons;

    [SerializeField] GameObject teamCardPrefab;
    [SerializeField] Transform teamCardContainer;
    [SerializeField] GameObject leaveTeamButton;


    private void OnEnable()
    {
        joinContainer.SetActive(true);
        lobbyContainer.SetActive(false);

        LobbyConnectionManager.OnLobbyJoined += OnLobbyJoined;;
        LobbyConnectionManager.OnLobbyExited += OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves += OnClientJoined;
        LobbyConnectionManager.OnLobbyOwnerLeft += LeaveLobby;

        TeamManager.OnTeamManagerCreated += CreateTeamCards;
    }

    private void OnDisable()
    {
        LobbyConnectionManager.OnLobbyJoined -= OnLobbyJoined;
        LobbyConnectionManager.OnLobbyExited -= OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves -= OnClientJoined;
        LobbyConnectionManager.OnLobbyOwnerLeft -= LeaveLobby;

        TeamManager.OnTeamManagerCreated -= CreateTeamCards;
    }

    public void UpdateLobbyProfiles()
    {
        CSteamID lobbyId = new CSteamID(LobbyConnectionManager.CurrentLobbyID);
        List<CSteamID> players = new List<CSteamID>();
        int lobbyMemberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
        for (int i = 0; i < lobbyMemberCount; i++)
        {
            CSteamID playerId = SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i);
            if (TeamManager.Instance == null || TeamManager.Instance.IsPlayerOwningSlot(playerId) == false)
                players.Add(playerId);
        }

        for (int i = 0; i < lobbyIcons.Count; i++)
        {
            CSteamID playerId = i < players.Count ? players[i] : CSteamID.Nil;
            lobbyIcons[i].CurrentSteamId = playerId;
        }

        startLobbyButton.interactable = InstanceFinder.IsServerStarted && players.Count == 0;
        leaveTeamButton.SetActive(TeamManager.Instance != null && TeamManager.Instance.IsPlayerOwningSlot(SteamUser.GetSteamID()));
    }

    private void CreateTeamCards()
    {
        foreach(var kvp in TeamManager.Instance.allTeams)
        { 
            GameObject tCard = Instantiate(teamCardPrefab, teamCardContainer).gameObject;
            UITeamCard teamCard = tCard.GetComponent<UITeamCard>();
            teamCard.SetCurrentTeam(kvp.Value);
        }
    }

    private void ClearTeamCards()
    {
        foreach (Transform t in teamCardContainer.transform)
        { 
            Destroy(t.gameObject);
        }
    }

    private void OnLobbyJoined()
    {
        joinContainer.SetActive(false);
        lobbyContainer.SetActive(true);
        id.text = LobbyConnectionManager.CurrentLobbyID.ToString();
        title.text = SteamMatchmaking.GetLobbyData(new CSteamID(LobbyConnectionManager.CurrentLobbyID), "LobbyName");
        startLobbyButton.interactable = InstanceFinder.IsServerStarted;
        UpdateLobbyProfiles();
    }

    private void OnClientJoined(CSteamID playerId)
    {
        UpdateLobbyProfiles();
    }

    private void OnLobbyExited()
    {
        ClearTeamCards();
        UpdateLobbyProfiles();
    }

    public void CreateLobby()
    {
        LobbyConnectionManager.CreateLobby();
    }

    public void JoinLobby(TMP_InputField input)
    {
        if (InstanceFinder.IsHostStarted) return;
        CSteamID steamID = new CSteamID(Convert.ToUInt64(input.text));
        LobbyConnectionManager.JoinLobbyByID(steamID);

    }

    public void StartGame()
    {
        NetworkSceneManager.LoadNetworkScene("Game", new string[] {"ConnectingScene"} );
    }

    public void LeaveLobby()
    {
        LobbyConnectionManager.LeaveLobby();
        joinContainer.SetActive(true);
        lobbyContainer.SetActive(false);
    }

    public void LeaveTeamRequest()
    {
        TeamManager.Instance.RequestLeaveTeamServerRPC(SteamUser.GetSteamID().m_SteamID);
    }

    public void SaveLobbyIdToClipboard()
    {
        GUIUtility.systemCopyBuffer = LobbyConnectionManager.CurrentLobbyID.ToString();
    }
}
