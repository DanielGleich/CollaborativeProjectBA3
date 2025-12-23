using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ScientistInputsHandler : InputHandler, PlayerInputs.IScientistControllsActions
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent<Vector2> onMove;
    [SerializeField] private UnityEvent onSwitchCameraView;
    [SerializeField] private UnityEvent onTryAttack;

    void OnEnable()
    {
        playerInputs.ScientistControlls.Enable();
        playerInputs.ScientistControlls.AddCallbacks(this);
        Cursor.visible = false;
    }
    void OnDisable()
    {
        playerInputs.ScientistControlls.Disable();
        playerInputs.ScientistControlls.RemoveCallbacks(this);
        Cursor.visible = true;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        onSwitchCameraView?.Invoke();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        onTryAttack?.Invoke();
    }
}
