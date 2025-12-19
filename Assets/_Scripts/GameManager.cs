using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkSingleton<GameManager>
{
    protected override bool _perClient => false;

    public static UnityEvent OnGameOver = new UnityEvent();
    public static UnityEvent OnGameWin = new UnityEvent();
    public static UnityEvent<NetworkObject> OnPlayerDied = new UnityEvent<NetworkObject>();
    public static UnityEvent OnLocalPlayerDied = new UnityEvent();
    private void Start()
    {
        //if (IsServerInitialized)
        //{ 
        //    PlayerManager.OnPlayerConnected.AddListener(RegisterPlayer);
        //    PlayerManager.OnPlayerDisconnected.AddListener(UnregisterPlayer);
        //}
    }

    //private void RegisterPlayer(NetworkObject player)
    //{

    //}

    //private void UnregisterPlayer(NetworkObject player)
    //{

    //}

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
