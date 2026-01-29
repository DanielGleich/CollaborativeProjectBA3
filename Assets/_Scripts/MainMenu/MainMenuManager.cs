using FishNet;
using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent OnLobbyJoin = new UnityEvent();


    private void OnEnable()
    {
        joinContainer.SetActive(true);
        lobbyContainer.SetActive(false);

        LobbyConnectionManager.OnLobbyJoined += OnLobbyJoined;;
        LobbyConnectionManager.OnLobbyExited += OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves += OnClientJoined;
        LobbyConnectionManager.OnLobbyJoinedViaFriendlist += JoinLobbyByFriendList;
        //LobbyConnectionManager.OnLobbyOwnerLeft += LeaveLobby;

        PlayerManager.OnPlayerConnected.AddListener(UpdateLobbyProfiles);
        PlayerManager.OnPlayerDisconnected.AddListener(UpdateLobbyProfiles);

        TeamManager.OnTeamManagerCreated += CreateTeamCards;
        TeamManager.OnTeamUpdate += UpdateLobbyProfiles;
    }

    private void OnDisable()
    {
        LobbyConnectionManager.OnLobbyJoined -= OnLobbyJoined;
        LobbyConnectionManager.OnLobbyExited -= OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves -= OnClientJoined;
        //LobbyConnectionManager.OnLobbyOwnerLeft -= LeaveLobby;

        PlayerManager.OnPlayerConnected.AddListener(UpdateLobbyProfiles);
        PlayerManager.OnPlayerDisconnected.AddListener(UpdateLobbyProfiles);

        TeamManager.OnTeamManagerCreated -= CreateTeamCards;
        TeamManager.OnTeamUpdate -= UpdateLobbyProfiles;
    }

    public void UpdateLobbyProfiles()
    {
        if (PlayerManager.Instance == null) return;

        List<CSteamID> players = new List<CSteamID>();
        foreach (int clientId in PlayerManager.Instance.AllPlayerConnections)
        {
            if (TeamManager.Instance != null && TeamManager.Instance.IsPlayerOwningSlot(clientId) == false)
            {
                if (PlayerManager.Instance != null && PlayerManager.Instance.AllPlayerSteamIds.ContainsKey(clientId))
                    players.Add(new CSteamID(PlayerManager.Instance.AllPlayerSteamIds[clientId]));
            }
        }

        for (int i = 0; i < lobbyIcons.Count; i++)
        {
            CSteamID playerId = i < players.Count ? players[i] : CSteamID.Nil;
            lobbyIcons[i].CurrentSteamId = playerId;
        }

        startLobbyButton.interactable = InstanceFinder.IsServerStarted && players.Count == 0;
        leaveTeamButton.SetActive(TeamManager.Instance != null && TeamManager.Instance.IsPlayerOwningSlot(InstanceFinder.NetworkManager.ClientManager.Connection.ClientId));
    }

    private void UpdateLobbyProfiles(int clientId)
    {
        UpdateLobbyProfiles();
    }

    public void CreateTeamCards()
    {
        ClearTeamCards();
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

    public void OnLobbyJoined()
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
        UpdateLobbyProfiles();
    }

    public void CreateLobby()
    {
        LobbyConnectionManager.CreateLobby();
    }

    private void JoinLobbyByFriendList()
    { 
        OnLobbyJoin?.Invoke();
    }

    public void JoinLobby(TMP_InputField input)
    {
        if (InstanceFinder.IsHostStarted || String.IsNullOrEmpty(input.text) || String.IsNullOrWhiteSpace(input.text)) return;
        CSteamID steamID = new CSteamID(Convert.ToUInt64(input.text));
        LobbyConnectionManager.JoinLobbyByID(steamID);
        OnLobbyJoin?.Invoke();
    }

    public void StartLobby()
    {
        NetworkSceneManager.LoadNetworkScene("Tutorial", new string[] {"ConnectingScene"} );
    }

    public void LeaveLobby()
    {
        LobbyConnectionManager.LeaveLobby();
        
        if (PlayerManager.Instance != null)
            Destroy(PlayerManager.Instance.gameObject);
        if (TeamManager.Instance != null)
            Destroy(TeamManager.Instance.gameObject);
        
        joinContainer.SetActive(true);
        lobbyContainer.SetActive(false);
    }

    public void LeaveTeamRequest()
    {
        TeamManager.Instance.RequestLeaveTeamServerRPC(InstanceFinder.NetworkManager.ClientManager.Connection.ClientId);
    }

    public void SaveLobbyIdToClipboard()
    {
        GUIUtility.systemCopyBuffer = LobbyConnectionManager.CurrentLobbyID.ToString();
    }
}
