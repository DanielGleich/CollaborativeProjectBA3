using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HingeJoint[] hingeJoints;

    [Header("Settings")]
    [SerializeField, Min(0)] private float motorSpeed = 600f;
    [SerializeField] private float motorForce = 300f;

    private void Awake() {
        foreach(var h in hingeJoints)
            h.useMotor = true;
    }

    public void SetInputDirection(Vector2 inputDirection)
    {
        foreach(var h in hingeJoints)
        {
            JointMotor motor = h.motor;
            motor.targetVelocity = Mathf.RoundToInt(inputDirection.y) * motorSpeed;
            motor.force = motorForce;
            h.motor = motor;
        }
    }
}
