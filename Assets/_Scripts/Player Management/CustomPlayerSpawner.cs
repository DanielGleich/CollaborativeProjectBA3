using UnityEngine;

public class CustomPlayerSpawner : NetworkSingleton<CustomPlayerSpawner>
{
    protected override bool _perClient { get; } = false;
    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerManager.Instance.ConnectToServerRPC();
    }
}
