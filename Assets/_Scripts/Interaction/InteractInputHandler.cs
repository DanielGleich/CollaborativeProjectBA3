using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractInputHandler : InputHandler, Inputs.IInteractActions
{
    [Header("Events")]
    [SerializeField] private UnityEvent onInteract;
    
    private Inputs inputs;
    void Awake()
    {
        inputs = new Inputs();
    }
    void OnEnable()
    {
        inputs.Interact.Enable();
        inputs.Interact.AddCallbacks(this);
    }
    void OnDisable()
    {
        inputs.Inspect.Disable();
        inputs.Interact.RemoveCallbacks(this);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            onInteract.Invoke();
    }
}
