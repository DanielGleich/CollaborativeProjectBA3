using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Triggers a Unity Event when interacted with
/// </summary>
public class SimpleInteractable : Interactable {
    [Header("Unity Events")]
    [SerializeField] private UnityEvent onInteract;

    public override void Interact()
    {
        onInteract.Invoke();
    }
}