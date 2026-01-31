using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System.Linq;

public static class NetworkSceneManager
{
    public static void LoadNetworkScene(string sceneToLoad, string[] scenesToUnload, bool replaceOption = true)
    {
        if (!InstanceFinder.IsServerStarted) { return; }

        SceneLoadData sceneLoadData = replaceOption ? new SceneLoadData(sceneToLoad) { ReplaceScenes = ReplaceOption.All } : new SceneLoadData(sceneToLoad);
        NetworkConnection[] connections = InstanceFinder.ServerManager.Clients.Values.ToArray();
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);

        if (scenesToUnload == null) return;
        foreach (string sceneName in scenesToUnload)
        {
            SceneUnloadData sceneUnloadData = new SceneUnloadData(sceneName);
            InstanceFinder.SceneManager.UnloadGlobalScenes(sceneUnloadData);
        }
    }
}
