using System;
using System.Collections;
using UnityEngine;

public class JumpHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private GroundedChecker groundedChecker;

    [Header("Settings")]
    [SerializeField] private float coyoteeTime = 0.25f;
    [SerializeField] private float inputDelay = 0.1f;


    public event Action OnJump;
    public bool CanJump => groundedChecker.IsGrounded || lastGroundedTime + coyoteeTime > Time.time;

    private float lastGroundedTime;

    void OnEnable()
    {
        groundedChecker.OnUpdateGrounded += UpdateLastGroundedTime;
    }
    void OnDisable()
    {
        groundedChecker.OnUpdateGrounded -= UpdateLastGroundedTime;
    }

    private void UpdateLastGroundedTime(bool isGrounded)
    {
        if(isGrounded)
            return;
        lastGroundedTime = Time.time - Time.deltaTime;
    }

    public void TryJump()
    {
        if(CanJump)
            Jump();
        else
        {
            StopAllCoroutines();
            StartCoroutine(InputDelayRoutine());
        }
    }
    private void Jump()
    {
        movement.Jump();
        OnJump?.Invoke();
    }
    private IEnumerator InputDelayRoutine()
    {
        float t = 0;
        while(t < inputDelay)
        {
            yield return null;
            t += Time.deltaTime;
            if(!CanJump)
                continue;
            Jump();
            break;
        }
    }
}
