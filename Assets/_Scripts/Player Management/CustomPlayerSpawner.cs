using System.Collections;
using UnityEngine;

public class CustomPlayerSpawner : NetworkSingleton<CustomPlayerSpawner>
{
    protected override bool _perClient { get; } = false;
    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(WaitForPlayerManager());
    }

    IEnumerator WaitForPlayerManager()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);
        PlayerManager.Instance.ConnectToServerRPC();
    }
}
