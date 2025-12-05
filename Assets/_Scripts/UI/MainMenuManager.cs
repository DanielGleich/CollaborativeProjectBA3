using FishNet;
using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] private TMP_Text _title, _id;
    [SerializeField] private GameObject _joinContainer, _lobbyContainer;
    [SerializeField] private Button _startLobbyButton;
    [SerializeField] private List<GameObject> _lobbyProfiles;


    private void OnEnable()
    {
        _joinContainer.SetActive(true);
        _lobbyContainer.SetActive(false);

        LobbyConnectionManager.OnLobbyJoined += OnLobbyJoined;
        LobbyConnectionManager.OnLobbyExited += OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves += OnClientJoined;

        LobbyConnectionManager.OnLobbyOwnerLeft += LeaveLobby;
    }

    private void OnDisable()
    {
        LobbyConnectionManager.OnLobbyJoined -= OnLobbyJoined;
        LobbyConnectionManager.OnLobbyExited -= OnLobbyExited;
        LobbyConnectionManager.OnClientJoinOrLeaves -= OnClientJoined;

        LobbyConnectionManager.OnLobbyOwnerLeft -= LeaveLobby;
    }

    void UpdateLobbyProfiles()
    {
        CSteamID lobbyId = new CSteamID(LobbyConnectionManager.CurrentLobbyID);
        List<CSteamID> players = new List<CSteamID>();
        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyId); i++)
        {
            players.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i));
        }

        for (int i = 0; i < _lobbyProfiles.Count; i++)
        {
            CSteamID playerId = i < players.Count ? players[i] : CSteamID.Nil;
            _lobbyProfiles[i].GetComponent<UILobbyProfile>().SetSteamId(playerId);
        }
    }

    private void OnLobbyJoined()
    {
        _joinContainer.SetActive(false);
        _lobbyContainer.SetActive(true);

        _id.text = LobbyConnectionManager.CurrentLobbyID.ToString();
        _title.text = SteamMatchmaking.GetLobbyData(new CSteamID(LobbyConnectionManager.CurrentLobbyID), "LobbyName");
        _startLobbyButton.interactable = InstanceFinder.IsServerStarted;
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
        _joinContainer.SetActive(true);
        _lobbyContainer.SetActive(false);
    }

    public void SaveLobbyIdToClipboard()
    {
        GUIUtility.systemCopyBuffer = LobbyConnectionManager.CurrentLobbyID.ToString();
    }
}
