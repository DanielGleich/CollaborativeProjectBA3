using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameEndScreen : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Button rematchButton;
    [SerializeField] Button teamSelectionButton;
    [SerializeField] TextMeshProUGUI rematchButtonText;

    [Header("Settings")]
    [SerializeField] float OnDeathDelay = 3;

    [Header("Events")]
    public UnityEvent OnPlayerWin = new UnityEvent();
    public UnityEvent OnPlayerLose = new UnityEvent();

    private readonly SyncList<NetworkConnection> playerReadyForRematch = new SyncList<NetworkConnection>();
    private readonly SyncVar<bool> noPlayerLeft = new SyncVar<bool>();

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
        OnPlayerLose?.Invoke();
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
        rematchButtonText.text = $"Rematch? ({playerReadyForRematch.Count}/{PlayerManager.Instance.AllPlayerConnections.Count})";
    }

    private void ActivateScreen()
    {
        StartCoroutine(OnEnableDelay());
    }

    IEnumerator OnEnableDelay()
    {
        yield return new WaitForSeconds(OnDeathDelay);
        teamSelectionButton.interactable = IsServerInitialized;
        rematchButtonText.text = $"Rematch? (0/{PlayerManager.Instance.AllPlayerConnections.Count})";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleWinLoseScreen(int winnerTeamId)
    { 
        int localClientId = InstanceFinder.ClientManager.Connection.ClientId;
        if (winnerTeamId == TeamMember.localTeamId)
        {
            OnPlayerWin?.Invoke();
            Debug.Log("Player w");
        }
        else
        {
            OnPlayerLose?.Invoke();
            Debug.Log("Player l");
        }
    }

    private void BackupPlayerLeaveScreen(int clientId)
    {
        RequestPlayerLeftServerRpc();
        OnPlayerLose?.Invoke();
        Debug.Log("Player l2");
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
