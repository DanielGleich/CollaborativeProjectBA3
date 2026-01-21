using System.Collections;
using UnityEngine;

public class CustomPlayerSpawner : NetworkSingleton<CustomPlayerSpawner>
{
    [Header("Settings")]
    [SerializeField] bool isLobby;
    protected override bool _perClient { get; } = false;
    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(WaitForPlayerManager());
    }

    IEnumerator WaitForPlayerManager()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);
        PlayerManager.Instance.SpawnPlayerObjectServerRpc(LocalConnection, isLobby);
    }
}
