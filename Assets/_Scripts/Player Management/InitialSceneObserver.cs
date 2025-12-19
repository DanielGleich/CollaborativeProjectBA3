using FishNet;
using FishNet.Connection;
using UnityEngine;

public class InitialSceneObserver : MonoBehaviour
{
    private void Start()
    {
        var networkManager = InstanceFinder.NetworkManager;
        if (networkManager != null)
            networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
    }

    private void OnDestroy()
    {
        var networkManager = InstanceFinder.NetworkManager;
        if (networkManager != null)
            networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
    }

    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        if (!asServer)
            return;

        if (!conn.Scenes.Contains(gameObject.scene))
            InstanceFinder.NetworkManager.SceneManager.AddConnectionToScene(conn, gameObject.scene);
    }
}