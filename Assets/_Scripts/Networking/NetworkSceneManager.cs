using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public static class NetworkSceneManager
{
    public static event Action<string> OnNetworkedSceneChanged;

    public static void LoadNetworkScene(string sceneToLoad, string[] scenesToUnload)
    {
        if (!InstanceFinder.IsServerStarted) { return; }

        SceneLoadData sceneLoadData = new SceneLoadData(sceneToLoad);
        NetworkConnection[] connections = InstanceFinder.ServerManager.Clients.Values.ToArray();
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
        foreach (string sceneName in scenesToUnload)
        {
            SceneUnloadData sceneUnloadData = new SceneUnloadData(sceneName);
            InstanceFinder.SceneManager.UnloadGlobalScenes(sceneUnloadData);
        }
    }

    private static void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        if (InstanceFinder.IsServerStarted)
        { 
            foreach (Scene s in obj.LoadedScenes)
            { 
                OnNetworkedSceneChanged?.Invoke(s.name);
            }
        }
        InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
    }
}
