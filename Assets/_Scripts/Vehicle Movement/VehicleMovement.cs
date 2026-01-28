using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component responsible for controlling the motor speed
/// </summary>
public class VehicleMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HingeJoint[] hingeJoints;

    [Header("Motor Settings (x = forward, y = backwards)")]
    [SerializeField, Min(0), Tooltip("x = forward, y = backwards")] private Vector2 motorSpeed = new(600f, 300f);
    [SerializeField, Tooltip("x = forward, y = backwards")] private Vector2 motorForce = new(25f, 25f);

    // Giving certain wheels extra boost is necessary because some wheels are smaller than others
    [Header("Extra Boost (for smaller wheels)")]
    [SerializeField] private HingeJoint[] extaBoostJoints;
    [SerializeField, Min(1)] private float boostMultiplier = 2;

    [Header("Settings")]
    [SerializeField, Tooltip("Decides if the vehicle stops, when the input direction (y) is 0")] private bool stopOnNoInput = true;
    [SerializeField, Range(-1,1), Tooltip("Decides if the vehicle alredy starts with a certain move direction")] private int startMovementInput = 0;

    public event Action<float> OnUpdateInputDirection;
    public float InputDirection {get; private set;}

    void OnValidate()
    {
        if(startMovementInput != 0 && stopOnNoInput)
            Debug.LogWarning("Having startMovementInput other than 0 only makes sens if you have stopOnNoInput disabled");
    }

    private void Awake() {
        foreach(var h in hingeJoints)
            h.useMotor = true;
    }
    void Start()
    {
        if(startMovementInput != 0)
            SetInputDirection(new(0, startMovementInput));
    }
    public void SetInputDirection(Vector2 inputDirection)
    {
        // Handle velocity
        InputDirection = Mathf.Round(inputDirection.y);
        float newTargetVelocity = InputDirection >= 0 ? motorSpeed.x : -motorSpeed.y;
        float newMotorForce = InputDirection >= 0? motorForce.x : motorForce.y;
        OnUpdateInputDirection?.Invoke(InputDirection);

        // Handle stoping the vehicle
        if(InputDirection == 0 && stopOnNoInput)
            newTargetVelocity = 0;

        foreach(HingeJoint h in hingeJoints)
        {
            JointMotor motor = h.motor;
            bool extraBoost = extaBoostJoints.Contains(h);
            motor.targetVelocity = extraBoost? newTargetVelocity * boostMultiplier : newTargetVelocity;
            motor.force = extraBoost? newMotorForce * boostMultiplier : newMotorForce;
            h.motor = motor;
        }
    }
}
