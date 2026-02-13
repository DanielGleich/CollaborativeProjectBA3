using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*<summary>
 * The GameEndScreen is the UI element which controls win or lose screen 
 * once the GameManager signals the GameEnd. In addition, it controls the 
 * rematch votes and also handles the forwarding into other scenes.
 * </summary>*/

public class GameEndScreen : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] List<GameObject> ObjectsToDisableOnStart;
    [SerializeField] Button rematchButton;
    [SerializeField] Button teamSelectionButton;
    [SerializeField] TextMeshProUGUI rematchButtonText;

    [Header("Settings")]
    [SerializeField] float OnDeathDelay = 3;

    [Header("Events")]
    public UnityEvent OnGameEnd = new UnityEvent();
    public UnityEvent OnPlayerWin = new UnityEvent();
    public UnityEvent OnPlayerLose = new UnityEvent();

    private readonly SyncList<NetworkConnection> playerReadyForRematch = new SyncList<NetworkConnection>();
    private readonly SyncVar<bool> noPlayerLeft = new SyncVar<bool>();

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
        foreach (GameObject o in ObjectsToDisableOnStart)
            o.SetActive(false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        noPlayerLeft.OnChange += NoPlayerLeft_OnChange;
        playerReadyForRematch.OnChange += PlayerReadyForRematch_OnChange;
        GameManager.OnGameOver.AddListener(ActivateScreen);
        GameManager.OnTeamWins.AddListener(HandleWinLoseScreen);
        PlayerManager.OnPlayerDisconnected.AddListener(BackupPlayerLeaveScreen);
        foreach (GameObject o in ObjectsToDisableOnStart)
            o.SetActive(false);
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
        OnGameEnd?.Invoke();
    }

    private void HandleWinLoseScreen(int winnerTeamId)
    { 
        int localClientId = InstanceFinder.ClientManager.Connection.ClientId;
        if (winnerTeamId == TeamMember.localTeamId)
        {
            OnPlayerWin?.Invoke();
        }
        else
        {
            OnPlayerLose?.Invoke();
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

