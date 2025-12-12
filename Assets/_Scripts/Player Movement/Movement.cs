using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public abstract class PlayerMovement : MonoBehaviour
{
    protected Camera cam;

    [field: Header("Movement Settings")]
    [field: SerializeField] public float Speed { get; protected set; } = 5;
    [SerializeField] protected float accelerationTime = 0.25f;

    public Vector2 InputDirection { get; protected set; }
    public Vector3 Velocity { get; protected set; }

    public event Action<Vector2> OnUpdateInputDirection;
    public event Action<Vector3> OnUpdateVelocity;

    void Awake()
    {
        if (!cam)
            cam = Camera.main;
    }
    public void SetInputDirection(Vector2 inputDirection)
    {
        this.InputDirection = inputDirection;
        OnUpdateInputDirection?.Invoke(inputDirection);
    }
    public void SetInputDirection(CallbackContext context)
    {
        this.InputDirection = context.ReadValue<Vector2>();
    }
    protected virtual void CalculateNewVelocity()
    {
        Velocity = Vector3.MoveTowards(Velocity, cam.GetFlatDirectionRelativeToView(InputDirection) * Speed, Time.deltaTime / accelerationTime * Speed);
        OnUpdateVelocity?.Invoke(Velocity);
    }
    /// <summary>
    /// Apply the velocity here
    /// </summary>
    protected abstract void Update();
    /// <summary>
    /// Rules for jumping, like checking if the character is grounded etc should be set by a differnt component that calls jump
    /// </summary>
    public abstract void Jump();
}

public static class CameraExtensions
{
    /// <summary>
    /// Converts a 2D input direction to a horizontal 3D direction relative to the camera's view.
    /// </summary>
    public static Vector3 GetFlatDirectionRelativeToView(this Camera cam, Vector2 input)
    {
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;

        Vector3 cameraRight = cam.transform.right;
        cameraRight.y = 0;
        cameraRight = cameraRight.normalized;

        return cameraForward * input.y + cameraRight * input.x;
    }
    public static Vector2 Get2dDirectionRelativeToCameraRotation(this Camera cam, Vector2 input)
    {
        return cam.transform.up * input.y + cam.transform.right * input.x;
    }
}