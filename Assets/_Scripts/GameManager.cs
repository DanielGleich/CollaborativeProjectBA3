using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum GameEndReason
{
    NONE,
    DESTROYED,
    DISCONNECT,
    OUTOFARENA
}

public class GameManager : NetworkSingleton<GameManager>
{
    protected override bool _perClient => false;

    [Header("Settings")]
    public bool IsTesting = true;
    public float countdownDuration = 3f;
    public readonly SyncTimer PreGameCountdown = new SyncTimer();
    private readonly SyncVar<bool> isGameStarted = new SyncVar<bool>();
    public readonly SyncVar<GameEndReason> gameEndReason = new SyncVar<GameEndReason>();

    public static event Action OnInitialized;
    public static UnityEvent OnGameStart = new UnityEvent();
    public static UnityEvent OnGameOver = new UnityEvent();
    public static UnityEvent<int> OnTeamWins = new UnityEvent<int>();

    private void Update()
    {
        if (PreGameCountdown.Paused == false)
            PreGameCountdown.Update();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        isGameStarted.Value = false;
        gameEndReason.Value = GameEndReason.NONE;
        PlayerManager.OnPlayerDisconnected.AddListener(TriggerDisconnectGameEnd);
        TeamManager.OnAllTeamsReady += SubscribeToPlayerDeaths;

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
    }

    [Server]
    private void SubscribeToPlayerDeaths()
    {
        foreach (var team in TeamManager.Instance.allTeams)
        {
            NetworkObject playerObject = PlayerManager.Instance.GetNetworkObjectByClientId(team.Value.scientistPlayerClientId);
            HealthNetworking health = playerObject?.transform.GetComponentInChildren<HealthNetworking>();
            if (health != null)
            {
                health.OnNetworkedDeath += HandlePlayerDeath;
            }
        }
    }

    [Server]
    private void HandlePlayerDeath(NetworkConnection playerConnection)
    {
        int winnerTeam = -1;
        int loserTeam = -1;
        Debug.Log("Death");

        foreach (var team in TeamManager.Instance.allTeams)
        {
            if (team.Value.scientistPlayerClientId == playerConnection.ClientId || team.Value.ratPlayerClientId == playerConnection.ClientId)
            {
                loserTeam = team.Key;
            }
            else
            { 
                winnerTeam = team.Key;
            }
        }

        if (IsTesting || (loserTeam >= 0 && winnerTeam >= 0))
        {
            if (gameEndReason.Value == GameEndReason.NONE)
                gameEndReason.Value = GameEndReason.DESTROYED;

            NotifyWinnerTeam(winnerTeam);
            NotifyGameOver();
        }
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
        if (IsTesting)
            SubscribeToPlayerDeaths();

        ChargingPadManagerNetworking.Instance?.InitializeManager();
        isGameStarted.Value = true;
        NotifyGameStart();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestGameEndReasonChangeServerRpc(GameEndReason reason)
    {
        gameEndReason.Value = reason;
    }

    [Server]
    private void TriggerDisconnectGameEnd(int clientId)
    {
        gameEndReason.Value = GameEndReason.DISCONNECT;
        NotifyGameOver();
    }

    [ObserversRpc]
    private void NotifyGameStart()
    {
        OnGameStart?.Invoke();
    }

    [ObserversRpc]
    private void NotifyGameOver()
    {
        OnGameOver?.Invoke();
    }

    [ObserversRpc]
    private void NotifyWinnerTeam(int teamId)
    { 
        OnTeamWins.Invoke(teamId);
    }
}
