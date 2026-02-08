using UnityEngine;

public class MovementCameraTilt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Movement movement;

    [Header("Settings")]
    [SerializeField, Tooltip("x = left & right; y = front & back")] private Vector2 tilt = new Vector2(2f, 0f);
    [SerializeField] float acceleration = 0.25f;
    private Vector2 currentTilt;
    private Vector3 currentVelocity;
    void OnValidate()
    {
        if(movement == null)
            movement = GetComponent<Movement>();
    }
    void Update()
    {
        float speedPercentage = movement.Velocity.magnitude / movement.Speed;
        currentTilt = Vector3.SmoothDamp(currentTilt, movement.InputDirection * speedPercentage, ref currentVelocity, acceleration, Mathf.Infinity, Time.deltaTime);
        transform.localEulerAngles = new Vector3(currentTilt.y * tilt.y, transform.localEulerAngles.y, currentTilt.x * tilt.x);
    }
}