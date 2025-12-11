using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class FPSLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform player;
    
    [Header("Settings")]
    [SerializeField] private Vector2 sensitivity = new Vector2(0.1f, 0.1f);
    [SerializeField, Range(0, 89.99f)] private float maxLookAngle = 60f;
    public Vector2 LookVelocity { get; private set; }

    public void Look(Vector2 look)
    {
        LookVelocity = look;
    }
    public void Look(CallbackContext context)
    {
        LookVelocity = context.ReadValue<Vector2>();
    }
    void Update()
    {
        float y = cameraTarget.localEulerAngles.x - Mathf.Clamp(LookVelocity.y * sensitivity.y, -90f, 90f);
        y = ClampAngle(y);
        cameraTarget.localEulerAngles = new Vector3(y, cameraTarget.localEulerAngles.y, cameraTarget.localEulerAngles.z);
        player.localEulerAngles += new Vector3(0, LookVelocity.x * sensitivity.x, 0);
    }
    public float ClampAngle(float angle)
    {
        if (angle < 180 )
            angle = Mathf.Clamp(angle, -90f, maxLookAngle);
        else if (angle >= 180)
            angle = Mathf.Clamp(angle, 360 - maxLookAngle, 360);
        return angle;
    }
}