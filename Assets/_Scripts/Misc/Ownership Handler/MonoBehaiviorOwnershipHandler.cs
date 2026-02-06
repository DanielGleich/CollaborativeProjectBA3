using System;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// Enables/& disables MonoBehaivor components based on ownership
/// </summary>
public class MonoBehaiviorOwnershipHandler : NetworkBehaviour {
    [SerializeField] private MonoBehaviour[] monoBehaviours;
    public override void OnStartClient()
    {
        Array.ForEach(monoBehaviours, m => m.enabled = IsOwner);
    }
}