using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractable : Interactable {
    [Header("Unity Events")]
    [SerializeField] private UnityEvent onInteract;

    public override void Interact()
    {
        onInteract.Invoke();
    }
}