using FishNet.Object;
using UnityEngine;

public class PlayerSpawnController : NetworkBehaviour

{
    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerManager.Instance.ConnectToServerRPC();
    }
}