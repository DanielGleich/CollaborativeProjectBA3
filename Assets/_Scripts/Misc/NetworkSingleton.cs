using FishNet.Object;
using System;
using UnityEngine;
/// <summary>
/// Abstract singleton pattern base class for networked singletons.
/// <br>Use this rather than NetworkBehavior to create singleton classes.</br>
/// </summary>
/// <typeparam name="T">Singleton component type</typeparam>
[DisallowMultipleComponent]
public abstract class NetworkSingleton<T> : NetworkBehaviour where T : Component
{
    /// <summary>
    /// The singleton instance of the class
    /// </summary>
    protected static T s_instance { get; private set; }

    /// <summary>
    /// When true, ignores singletons of the same type owned by other clients.
    /// </summary>
    protected virtual bool _perClient { get; } = true;

    /// <summary>
    /// In the case of multiple instance, whether the GameObject should be destroyed rather than just the component.
    /// <br>Set to true by default</br>
    /// </summary>
    protected virtual bool _destroyEntireGameObject { get; } = true;

    /// <summary>
    /// Returns true if this singleton exists.
    /// </summary>
    public static bool Exists => s_instance != null;

    /*    private void Awake()
        {
            s_instance = null;
        }*/

    public override void OnStartClient()
    {
        //if the instance is per client then don't set the instance claue if you are not the owner
        if (_perClient)
        {
            if (!IsOwner) { return; }
        }

        //Check wether generic type matches component type
        if (GetType() != typeof(T))
        {
            throw new Exception($"Generic type does not match component type:({GetType()}) in the singleton definition");
        }

        //Handles instance already existing
        if (s_instance != null)
        {
            UnityEngine.Debug.LogWarning($"Two instances of singleton({GetType()}) exist. Destroying the new instance");
            return;
        }

        //Setup instance
        s_instance = this as T;
    }
}