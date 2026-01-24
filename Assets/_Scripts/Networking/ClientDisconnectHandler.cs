using FishNet;
using FishNet.Managing.Client;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientDisconnectHandler : MonoBehaviour
{
    private void OnEnable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState; ;
    }

    private void OnDisable()
    {
        if (InstanceFinder.ClientManager != null)
            InstanceFinder.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState; ;
    }

    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            SceneManager.LoadScene("ConnectingScene");
        }
    }
}
