using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HingeJoint[] hingeJoints;

    [Header("Motor Settings (x = forward, y = backwards)")]
    [SerializeField, Min(0), Tooltip("x = forward, y = backwards")] private Vector2 motorSpeed = new(600f, 300f);
    [SerializeField, Tooltip("x = forward, y = backwards")] private Vector2 motorForce = new(25f, 25f);

    private void Awake() {
        foreach(var h in hingeJoints)
            h.useMotor = true;
    }

    public void SetInputDirection(Vector2 inputDirection)
    {
        int newDirection = Mathf.RoundToInt(inputDirection.y);
        float newTargetVelocity = (newDirection >= 0 ? motorSpeed.x : motorSpeed.y) * newDirection;
        float newMotorForce = newDirection >= 0? motorForce.x : motorForce.y;

        foreach(HingeJoint h in hingeJoints)
        {
            JointMotor motor = h.motor;
            motor.targetVelocity = newTargetVelocity;
            motor.force = newMotorForce;
            h.motor = motor;
        }
    }
}
