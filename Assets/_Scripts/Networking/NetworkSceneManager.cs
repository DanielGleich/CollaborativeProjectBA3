using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System.Linq;

public static class NetworkSceneManager
{
    public static void LoadNetworkScene(string sceneToLoad, string[] scenesToUnload)
    {
        if (!InstanceFinder.IsServerStarted) { return; }

        SceneLoadData sceneLoadData = new SceneLoadData(sceneToLoad);
        NetworkConnection[] connections = InstanceFinder.ServerManager.Clients.Values.ToArray();
        //InstanceFinder.SceneManager.LoadConnectionScenes(connections, sceneLoadData);
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);

        foreach (string sceneName in scenesToUnload)
        {
            SceneUnloadData sceneUnloadData = new SceneUnloadData(sceneName);
            //InstanceFinder.SceneManager.UnloadConnectionScenes(connections, sceneUnloadData);
            InstanceFinder.SceneManager.UnloadGlobalScenes(sceneUnloadData);
        }
    }
}
