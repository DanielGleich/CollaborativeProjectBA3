using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

public class LobbyManager : NetworkSingleton<LobbyManager>
{
    [Header("Settings")]
    public float readyTimerDuration = 3f;
    protected override bool _perClient { get; } = false;
    public readonly SyncList<NetworkConnection> AllReadyPlayers = new SyncList<NetworkConnection>();
    public readonly SyncTimer CountdownTimer = new SyncTimer();
    public bool isCountdownActive { get; private set; } = false;

    public static event Action OnInitialized;
    public event Action OnCountdownStart;
    public event Action OnCountdownCancel;

    private void OnEnable()
    {
        LobbyReadyInteraction.OnLobbyReadyInteraction += OnLocalRequestHandle;
    }

    private void OnDisable()
    {
        LobbyReadyInteraction.OnLobbyReadyInteraction -= OnLocalRequestHandle;
        if (IsServerInitialized)
            CountdownTimer.OnChange -= CountdownTimer_OnChange;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        CountdownTimer.OnChange += CountdownTimer_OnChange;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnInitialized?.Invoke();
    }

    [Client]
    private void OnLocalRequestHandle()
    {
        RequestReadyStatusToggleServerRpc(LocalConnection);
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestReadyStatusToggleServerRpc(NetworkConnection playerConnection)
    {
        if (AllReadyPlayers.Contains(playerConnection))
        {
            if (AllReadyPlayers.Count == PlayerManager.Instance.AllPlayerConnections.Count)
                CancelCountdown();

            AllReadyPlayers.Remove(playerConnection);
        }
        else
        {
            AllReadyPlayers.Add(playerConnection);
        }

        if (AllReadyPlayers.Count == PlayerManager.Instance.AllPlayerConnections.Count)
        {
            StartCountdown();
        }
    }

    [Server]
    private void StartCountdown()
    {
        CountdownTimer.StartTimer(readyTimerDuration);
        isCountdownActive = true;
        NotifyCountdownStart();
    }

    [Server]
    private void CancelCountdown()
    {
        CountdownTimer.StopTimer();
        isCountdownActive = false;
        NotifyCountdownCancel();
    }

    [ObserversRpc]
    private void NotifyCountdownStart()
    {
        OnCountdownStart?.Invoke();
    }

    [ObserversRpc]
    private void NotifyCountdownCancel()
    { 
        OnCountdownCancel?.Invoke();
    }

    private void CountdownTimer_OnChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (asServer == false) return;
        if (op == SyncTimerOperation.Finished)
        {
            PlayerManager.Instance.ResetManagerForSceneChange();
            TeamManager.Instance.ResetPlayerReadyStates();
            NetworkSceneManager.LoadNetworkScene("Game", null);
        }
    }

    private void Update()
    {
        CountdownTimer.Update();
    }
}
