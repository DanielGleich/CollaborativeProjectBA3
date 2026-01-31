using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameEndScreen : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GameObject uiParent;
    [SerializeField] GameObject winnerUI;
    [SerializeField] GameObject loserUI;
    [SerializeField] Button rematchButton;
    [SerializeField] Button teamSelectionButton;
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] TextMeshProUGUI rematchButtonText;
    [SerializeField] TextMeshProUGUI rematchCounter;

    private readonly SyncList<NetworkConnection> playerReadyForRematch = new SyncList<NetworkConnection>();
    private readonly SyncVar<bool> noPlayerLeft = new SyncVar<bool>();

    private void Awake()
    {
        uiParent.SetActive(false);
    }

    private void OnEnable()
    {        
        GameManager.OnGameOver.AddListener(ActivateScreen);
        GameManager.OnTeamWins.AddListener(HandleWinLoseScreen);
        PlayerManager.OnPlayerDisconnected.AddListener(BackupPlayerLeaveScreen);
        noPlayerLeft.OnChange += NoPlayerLeft_OnChange;
    }


    private void OnDisable()
    {
        GameManager.OnGameOver.RemoveListener(ActivateScreen);
        GameManager.OnTeamWins.RemoveListener(HandleWinLoseScreen);
        PlayerManager.OnPlayerDisconnected.RemoveListener(BackupPlayerLeaveScreen);        
        noPlayerLeft.OnChange -= NoPlayerLeft_OnChange;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        noPlayerLeft.Value = true;
        playerReadyForRematch.OnChange += PlayerReadyForRematch_OnChange;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.Instance.gameEndReason.OnChange += GameEndReason_OnChange;
    }

    private void GameEndReason_OnChange(GameEndReason prev, GameEndReason next, bool asServer)
    {
        if (uiParent.activeSelf && loserUI.activeSelf)
        {
            switch (GameManager.Instance?.gameEndReason.Value)
            {
                case GameEndReason.DESTROYED:
                    deathText.text = "Your RattleBot got destroyed!";
                    break;
                case GameEndReason.DISCONNECT:
                    deathText.text = "Your team mate left the game!";
                    break;
                case GameEndReason.OUTOFARENA:
                    deathText.text = "Your RattleBot left the arena!";
                    break;
            }
        }
    }

    private void NoPlayerLeft_OnChange(bool prev, bool next, bool asServer)
    {
        rematchButton.interactable = next;
    }

    [Server]
    private void PlayerReadyForRematch_OnChange(SyncListOperation op, int index, NetworkConnection oldItem, NetworkConnection newItem, bool asServer)
    {
        if (noPlayerLeft.Value && playerReadyForRematch.Count == PlayerManager.Instance.AllPlayerConnections.Count)
        { 
            NotifyRematch();
            NetworkSceneManager.LoadNetworkScene("Tutorial");
        }
        NotifyPlayerRematchReady();
    }

    [ObserversRpc]
    private void NotifyPlayerRematchReady()
    { 
        rematchButtonText.text = playerReadyForRematch.Contains(LocalConnection) ? "Waiting for Rematch" : "Rematch?";
        rematchCounter.text = $"({playerReadyForRematch.Count}/{PlayerManager.Instance.AllPlayerConnections.Count})";
    }

    private void ActivateScreen()
    {
        uiParent.SetActive(true);
        teamSelectionButton.interactable = IsServerInitialized;
        rematchCounter.text = $"({playerReadyForRematch.Count}/{PlayerManager.Instance.AllPlayerConnections.Count})";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleWinLoseScreen(int winnerTeamId)
    { 
        int localClientId = InstanceFinder.ClientManager.Connection.ClientId;
        if (winnerTeamId == TeamMember.localTeamId)
        {
            winnerUI.SetActive(true);
        }
        else
        { 
            loserUI.SetActive(true);
            switch (GameManager.Instance?.gameEndReason.Value)
            {
                case GameEndReason.DESTROYED:
                    deathText.text = "Your RattleBot got destroyed!";
                break;
                case GameEndReason.DISCONNECT:
                    deathText.text = "Your team mate left the game!";
                break;
                case GameEndReason.OUTOFARENA:
                    deathText.text = "Your RattleBot left the arena!";
                break;
            }
        }
    }

    private void BackupPlayerLeaveScreen(int clientId)
    {
        RequestPlayerLeftServerRpc();
        if (deathText.text == string.Empty)
        { 
            loserUI.SetActive(true);
            deathText.text = "A player left the game";
        }
    }

    public void RequestRematch()
    {
        if (playerReadyForRematch.Contains(LocalConnection))
            UntriggerRematch(LocalConnection);
        else
            TriggerRematch(LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerRematch(NetworkConnection c)
    {
        playerReadyForRematch.Add(c);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UntriggerRematch(NetworkConnection c)
    {
        playerReadyForRematch.Remove(c);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerLeftServerRpc()
    {
        noPlayerLeft.Value = false;
    }

    [ObserversRpc]
    private void NotifyRematch()
    {
        PlayerManager.Instance.ResetManagerForSceneChange();
        TeamManager.Instance.ResetPlayerReadyStates();
    }

    public void MoveToTeamSelection()
    {
        RequestPlayerLeftServerRpc();
        if (playerReadyForRematch.Contains(LocalConnection))
            UntriggerRematch(LocalConnection);

        PlayerManager.Instance.ResetManagerForSceneChange();
        TeamManager.Instance.ResetPlayerReadyStates();
        NetworkSceneManager.LoadNetworkScene("ConnectingScene");
    }

    public void MoveToMainMenu()
    {
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.StopConnection(true);
        InstanceFinder.ClientManager.StopConnection();
        LobbyConnectionManager.LeaveLobby();
        UnityEngine.SceneManagement.SceneManager.LoadScene("ConnectingScene");
    }
}
