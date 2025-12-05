using FishNet.Object;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InputController : Singleton<InputController>
{
    Movement _playerMovement;
    PlayerLook _playerLook;
    private InputSystem_Actions _inputActions;
    private InputAction _movementAction;
    private InputAction _lookAction;

    private void Awake()
    {
        base.Awake();
        _inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        PlayerManager.OnLocalPlayerConnected.AddListener(RegisterLocalPlayer);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _movementAction = _inputActions.Player.Move;
        _movementAction.performed += Movement;
        _movementAction.canceled += Movement;
        _movementAction.Enable();

        _lookAction = _inputActions.Player.Look;
        _lookAction.performed += Looking;
        _lookAction.Enable();
    }

    private void OnDisable()
    {
        _movementAction.performed -= Movement;
        _movementAction.canceled -= Movement;
        _movementAction?.Disable();

        _lookAction.performed -= Looking;
        _lookAction?.Disable();
    }

    private void RegisterLocalPlayer(NetworkObject player)
    {
        _playerMovement = player.GetComponent<Movement>();
        _playerLook = player.GetComponent<PlayerLook>();
        _playerLook.SetLocalPlayerCamera();
    }

    private void Movement(InputAction.CallbackContext ctx)
    {
        if (_playerMovement == null) return;
        Vector2 input = ctx.ReadValue<Vector2>();
        _playerMovement.SetMovement(new Vector3(input.x, 0, input.y));
    }

    private void Looking(InputAction.CallbackContext ctx)
    {
        if (_playerLook == null) return;
        Vector2 mouseDelta = ctx.ReadValue<Vector2>();
        _playerLook.ApplyLook(mouseDelta);
    }
}
