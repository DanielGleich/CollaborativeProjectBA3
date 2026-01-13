using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ScientistInputHandler : MonoBehaviour, PlayerInputs.IFPSControllsActions
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent<Vector2> onMove;
    [SerializeField] private UnityEvent<Vector2> onLook;
    [SerializeField] private UnityEvent onJump;
    [SerializeField] private UnityEvent onInteract;

    private PlayerInputs playerInputs;
    private void Awake() {
        playerInputs = new PlayerInputs();
    }
    void OnEnable()
    {
        playerInputs.FPSControlls.Enable();
        playerInputs.FPSControlls.AddCallbacks(this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnDisable()
    {
        playerInputs.FPSControlls.Disable();
        playerInputs.FPSControlls.RemoveCallbacks(this);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
            onInteract?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
            onJump?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        onLook?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }
}
