using System.Linq;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HingeJoint[] hingeJoints;

    [Header("Motor Settings (x = forward, y = backwards)")]
    [SerializeField, Min(0), Tooltip("x = forward, y = backwards")] private Vector2 motorSpeed = new(600f, 300f);
    [SerializeField, Tooltip("x = forward, y = backwards")] private Vector2 motorForce = new(25f, 25f);

    [Header("Extra Boost")]
    [SerializeField] private HingeJoint[] extaBoostJoints;
    [SerializeField, Min(1)] private float boostMultiplier = 2;

    [Header("Settings")]
    [SerializeField, Tooltip("Decides if the vehicle stops, when the input direction (y) is 0")] private bool stopOnNoInput = true;
    [SerializeField, Range(-1,1), Tooltip("Decides if the vehicle alredy starts with a certain move direction")] private int startMovementInput = 0;

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
        int newDirection = Mathf.RoundToInt(inputDirection.y);
        Debug.Log($"InputDirection");
        float newTargetVelocity = newDirection >= 0 ? motorSpeed.x : -motorSpeed.y;
        float newMotorForce = newDirection >= 0? motorForce.x : motorForce.y;

        // Handle stoping the vehicle
        if(newDirection == 0 && stopOnNoInput)
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
