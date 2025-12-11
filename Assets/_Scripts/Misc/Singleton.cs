using System;
using UnityEngine;

/// <summary>
/// Abstract singleton pattern base class.
/// <br>Use this rather than MonoBehavior to create singleton classes.</br>
/// </summary>
/// <typeparam name="T">Singleton component type</typeparam>
[DisallowMultipleComponent]
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    /// <summary>
    /// The singleton instance of the class
    /// </summary>
    protected static T s_instance { get; private set; }

    public static T Instance => s_instance;
    /// <summary>
    /// Whether the singleton should be destroyed on load
    /// <br>Set to false by default</br>
    /// </summary>
    protected virtual bool _dontDestroyOnLoad { get; } = false;

    /// <summary>
    /// In the case of multiple instnyance, whether the GameObject should be destroyed rather than just the component.
    /// <br>Set to true by default</br>
    /// </summary>
    protected virtual bool _destroyEntireGameObject { get; } = true;

    protected virtual void Awake()
    {
        //Check wether generic type matches component type
        if (GetType() != typeof(T))
        {
            throw new Exception($"Generic type does not match component type:({GetType()}) in the singleton definition");
        }

        //Handles instance already existing
        if (s_instance != null)
        {
            UnityEngine.Debug.LogWarning($"Two instances of singleton({GetType()}) exist. Destroying the new instance");
            
            if (_destroyEntireGameObject)
            {
                Destroy(gameObject);
                return;
            }
            Destroy(this);
            return;
        }

        //Setup instance
        s_instance = this as T;
        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this);
        }
    }
}
