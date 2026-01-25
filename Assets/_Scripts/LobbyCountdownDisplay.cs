using FishNet;
using TMPro;
using UnityEngine;

public class LobbyCountdownDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hintField;
    [SerializeField] GameObject countdownBackground;
    [SerializeField] TextMeshProUGUI countdownField;

    LobbyManager lobbyManager;

    private void OnEnable()
    {
        LobbyManager.OnInitialized += Subscribe;
        HideCountdown();
    }

    private void Subscribe()
    {
        lobbyManager = LobbyManager.Instance;
        PlayerManager.OnPlayerConnected.AddListener(UpdateText);
        lobbyManager.OnCountdownStart += ShowCountdown;
        lobbyManager.OnCountdownCancel += HideCountdown;
        lobbyManager.AllReadyPlayers.OnChange += AllReadyPlayers_OnChange;

        UpdateText();
    }


    private void OnDisable()
    {
        LobbyManager.OnInitialized -= Subscribe;
        if (lobbyManager == null) return;
        PlayerManager.OnPlayerDisconnected.AddListener(UpdateText);
        PlayerManager.OnPlayerDisconnected.AddListener(UpdateText);
        lobbyManager.OnCountdownStart -= ShowCountdown;
        lobbyManager.OnCountdownCancel -= HideCountdown;
        lobbyManager.AllReadyPlayers.OnChange -= AllReadyPlayers_OnChange;
    }
    private void AllReadyPlayers_OnChange(FishNet.Object.Synchronizing.SyncListOperation op, int index, FishNet.Connection.NetworkConnection oldItem, FishNet.Connection.NetworkConnection newItem, bool asServer)
    {
        UpdateText();
    }

    private void UpdateText(int p)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        string txt = "";
        txt += lobbyManager.AllReadyPlayers.Contains(InstanceFinder.ClientManager.Connection) ? "Ready" : "Press Enter to be ready";
        txt += $" ({lobbyManager.AllReadyPlayers.Count}/{PlayerManager.Instance?.AllPlayerConnections.Count})";
        hintField.text = txt;
    }

    private void ShowCountdown()
    {
        countdownField.gameObject.SetActive(true);
        countdownBackground.SetActive(true);
    }

    private void HideCountdown()
    {
        countdownField.gameObject.SetActive(false);
        countdownBackground.SetActive(false);
    }

    private void Update()
    {
        if (lobbyManager != null)
            countdownField.text = Mathf.CeilToInt(lobbyManager.CountdownTimer.Remaining).ToString();
    }
}
