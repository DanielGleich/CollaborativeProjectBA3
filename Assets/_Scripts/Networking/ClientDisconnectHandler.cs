using FishNet;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientDisconnectHandler : MonoBehaviour
{
    private void OnEnable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += HandleClientState;
    }

    private void OnDisable()
    {
        if (InstanceFinder.ClientManager != null)
            InstanceFinder.ClientManager.OnClientConnectionState -= HandleClientState;
    }

    private void HandleClientState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            SceneManager.LoadScene("ConnectingScene");
        }
    }
}
