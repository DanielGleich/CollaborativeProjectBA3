using System;
using System.Collections;
using UnityEngine;

public class CharacterControllerJumpHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterControllerMovement characterControllerMovement;
    [SerializeField] private CharacterController characterController;

    [Header("Settings")]
    [SerializeField] private float coyoteTime = 0.25f;
    [SerializeField] private float inputDelay = 0.25f;
    private float coyoteTimer;

    private Coroutine inputDelayRoutine;
    public Action OnJump;

    private bool canJump;
    public Action<bool> OnCanJump;
    public bool CanJump
    {
        get => canJump;
        private set
        {
            if (canJump == value)
                return;
            canJump = value;
            OnCanJump?.Invoke(canJump);
        }
    }
    void OnValidate()
    {
        if (characterControllerMovement == null)
            characterControllerMovement = GetComponent<CharacterControllerMovement>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
        coyoteTimer = characterController.isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
        CanJump = coyoteTimer > 0|| characterController.isGrounded;
    }
    public void Jump()
    {
        if (inputDelayRoutine != null)
            StopCoroutine(inputDelayRoutine);
        inputDelayRoutine = StartCoroutine(InputDelayRoutine());
    }
    IEnumerator InputDelayRoutine()
    {
        float time = 0;
        while (time <= inputDelay)
        {
            yield return null;
            time += Time.deltaTime;
            if (!CanJump)
                continue;
            characterControllerMovement.Jump();
            coyoteTimer = -coyoteTime;
            OnJump?.Invoke();
        }
    }
}