using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CombatInputHandler : InputHandler, Inputs.ICombatActions
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent<bool> onStartShoot;
    [SerializeField] private UnityEvent onSimpleShoot;
    [SerializeField] private UnityEvent onReload;
    [SerializeField] private UnityEvent<bool> onAim;
    private Inputs inputs;
    void Awake()
    {
        inputs = new Inputs();
    }
    void OnEnable()
    {
        inputs.Combat.Enable();
        inputs.Combat.AddCallbacks(this);
    }
    void OnDisable()
    {
        inputs.Combat.Disable();
        inputs.Combat.RemoveCallbacks(this);
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
            onAim?.Invoke(true);
        else if (context.canceled)
            onAim?.Invoke(false);
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
            onReload?.Invoke();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onSimpleShoot?.Invoke();
            onStartShoot?.Invoke(true);
        }
        else if (context.canceled)
            onStartShoot?.Invoke(false);
    }
}
