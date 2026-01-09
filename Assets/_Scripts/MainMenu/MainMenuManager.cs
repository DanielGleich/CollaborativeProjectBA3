using FishNet;
using FishNet.Connection;
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
        TeamManager.OnTeamUpdate += UpdateLobbyProfiles;
    }

    private void OnDisable()
    {
        LobbyConnectionManager.OnLobbyJoined -= OnLobbyJoined;
        LobbyConnectionManager.OnLobbyExited -= OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves -= OnClientJoined;
        LobbyConnectionManager.OnLobbyOwnerLeft -= LeaveLobby;

        TeamManager.OnTeamManagerCreated -= CreateTeamCards;
        TeamManager.OnTeamUpdate -= UpdateLobbyProfiles;
    }

    public void UpdateLobbyProfiles()
    {
        List<CSteamID> players = new List<CSteamID>();
        foreach (var kvp in InstanceFinder.NetworkManager.ClientManager.Clients)
        {
            if (TeamManager.Instance != null && TeamManager.Instance.IsPlayerOwningSlot(kvp.Value) == false)
            { 
                if(ulong.TryParse(kvp.Value.GetAddress(), out ulong playerID))
                    players.Add(new CSteamID(playerID));
                else 
                    Debug.Log("Parsing didnt work");
            }
        }

        Debug.Log($"{players.Count}");
        for (int i = 0; i < lobbyIcons.Count; i++)
        {
            CSteamID playerId = i < players.Count ? players[i] : CSteamID.Nil;
            lobbyIcons[i].CurrentSteamId = playerId;
        }

        startLobbyButton.interactable = InstanceFinder.IsServerStarted && players.Count == 0;
        leaveTeamButton.SetActive(TeamManager.Instance != null && TeamManager.Instance.IsPlayerOwningSlot(InstanceFinder.NetworkManager.ClientManager.Connection));
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
        TeamManager.Instance.RequestLeaveTeamServerRPC(InstanceFinder.NetworkManager.ClientManager.Connection);
    }

    public void SaveLobbyIdToClipboard()
    {
        GUIUtility.systemCopyBuffer = LobbyConnectionManager.CurrentLobbyID.ToString();
    }
}
