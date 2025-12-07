using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementInputHandler : InputHandler, Inputs.IMovementActions
{
    [Header("Events")]
    [SerializeField] private UnityEvent onJump;
    [SerializeField] private UnityEvent<Vector2> onLook;
    [SerializeField] private UnityEvent<Vector2> onMove;
    [SerializeField] private UnityEvent onExit;
    private Inputs inputs;
    void Awake()
    {
        inputs = new Inputs();
    }
    void OnEnable()
    {
        inputs.Movement.Enable();
        inputs.Movement.AddCallbacks(this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnDisable()
    {
        inputs.Movement.Disable();
        inputs.Movement.RemoveCallbacks(this);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            onJump.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        onLook.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        onMove.Invoke(context.ReadValue<Vector2>());
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
            onExit?.Invoke();
    }
}
