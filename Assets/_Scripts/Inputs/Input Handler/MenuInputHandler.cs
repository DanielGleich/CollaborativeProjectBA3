using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MenuInputHandler : InputHandler, PlayerInputs.IMenuInputsActions
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent toggleMenu;
    void OnEnable()
    {
        playerInputs.MenuInputs.Enable();
        playerInputs.MenuInputs.AddCallbacks(this);
    }
    void OnDisable()
    {
        playerInputs.MenuInputs.Disable();
        playerInputs.MenuInputs.RemoveCallbacks(this);
    }
    public void OnToggleMenu(InputAction.CallbackContext context)
    {
        if(context.performed)
            toggleMenu?.Invoke();
    }
}