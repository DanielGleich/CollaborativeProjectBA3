using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkSingleton<GameManager>
{
    protected override bool _perClient => false;

    [Header("Settings")]
    public bool IsTesting = true;
    public float countdownDuration = 3f;

    public static event Action OnInitialized;
    public static UnityEvent OnGameStart = new UnityEvent();
    public static UnityEvent OnGameOver = new UnityEvent();
    public static UnityEvent OnGameWin = new UnityEvent();
    public static UnityEvent<NetworkObject> OnPlayerDied = new UnityEvent<NetworkObject>();
    public static UnityEvent OnLocalPlayerDied = new UnityEvent();

    public readonly SyncTimer PreGameCountdown = new SyncTimer();

    private readonly SyncVar<bool> isGameStarted = new SyncVar<bool>();

    private void Start()
    {
        //if (IsServerInitialized)
        //{ 
        //    PlayerManager.OnPlayerConnected.AddListener(RegisterPlayer);
        //    PlayerManager.OnPlayerDisconnected.AddListener(UnregisterPlayer);
        //}
    }

    private void Update()
    {
        if (PreGameCountdown.Paused == false)
            PreGameCountdown.Update();
    }


    public override void OnStartServer()
    {
        base.OnStartServer();
        isGameStarted.Value = false;

        if (IsTesting == false)
        { 
            TeamManager.OnAllTeamsReady += StartCountdown;
            PreGameCountdown.OnChange += PreGameCountdown_OnChange;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnInitialized?.Invoke();

        if (IsTesting && IsServerInitialized)
            StartGame();
    }

    private void PreGameCountdown_OnChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished)
        {
            StartGame();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartCountdown()
    {
        if (countdownDuration >= 0)
        {
            PreGameCountdown.StartTimer(countdownDuration);
        }
        else
        {
            Debug.LogWarning("countdownDuration is not set - Game will not start!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGame()
    {
        if (isGameStarted.Value) return;            
        ChargingPadManagerNetworking.Instance?.InitializeManager();
        isGameStarted.Value = true;
        Debug.Log("Server Gamestart");
        NotifyGameStart();
    }

    [ObserversRpc]
    private void NotifyGameStart()
    {
        Debug.Log("Client Gamestart");
        OnGameStart?.Invoke();
    }

    private void CheckLosingCondition()
    { 
        //PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        //if (playerManager != null)
        //{
        //    foreach (KeyValuePair<NetworkObject, DamageableController> playerHealthEntry in _allPlayerHealths)
        //    {
        //        if (playerHealthEntry.Value != null)
        //        {
        //            if (!playerHealthEntry.Value.IsDead)
        //            {
        //                return;
        //            }
        //        }
        //        else 
        //        {
        //            Debug.Log($"PlayerObject {playerHealthEntry.Value.gameObject.name} has no Damageable-Component");
        //        }
        //    }
        //    HandleGameOver();
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    public void WinGameServerRPC()
    {
        HandleWin();
    }

    [ObserversRpc]
    private void HandleWin()
    {
        OnGameWin?.Invoke();
    }


    [ObserversRpc]
    private void RpcHandleGameOver()
    {
        OnGameOver?.Invoke();
    }

    private void HandleGameOver()
    {
        if (IsServerInitialized)
        {
            RpcHandleGameOver(); 
        }
    }
}
