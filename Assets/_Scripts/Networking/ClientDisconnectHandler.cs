using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientDisconnectHandler : NetworkBehaviour
{
    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        UnityEngine.SceneManagement.SceneManager.LoadScene("ConnectingScene");
    }
}
