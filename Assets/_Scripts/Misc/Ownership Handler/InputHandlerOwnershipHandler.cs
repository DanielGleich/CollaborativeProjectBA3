using FishNet.Object;
using UnityEngine;

public class InputHandlerOwnershipHandler : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private InputHandler[] inputHandlers;

    [ContextMenu("Get InputHandlers in children")]
    private void GetInputHandlersInChildren()
    {
        inputHandlers = GetComponentsInChildren<InputHandler>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        foreach(var i in inputHandlers)
            i.enabled = IsOwner;
    }
}