using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NetworkSceneManager
{
    public static void LoadNetworkScene(string sceneToLoad)
    {
        if (!InstanceFinder.IsServerStarted) return;

        CleanupDestroyedNetworkObjects();

        SceneLoadData sceneLoadData = new SceneLoadData(sceneToLoad) { ReplaceScenes = ReplaceOption.All };
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }

    private static void CleanupDestroyedNetworkObjects()
    {
        Dictionary<int, NetworkObject> objects = InstanceFinder.ServerManager.Objects.Spawned;
        var keysToRemove = objects.Keys.Where(k =>
            objects[k] == null ||
            objects[k].gameObject == null
        ).ToArray();

        foreach (var key in keysToRemove)
        {
            Debug.LogWarning($"Removing destroyed NetworkObject with ID: {key}");
            objects.Remove(key);
        }
    }
}
