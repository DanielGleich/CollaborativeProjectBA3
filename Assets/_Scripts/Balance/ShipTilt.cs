using UnityEngine;

public class ShipTilt : MonoBehaviour {
    [Header("References")]
    [SerializeField] private BallastScale ballastScale;

    [Header("Settings")]
    [SerializeField] private float ballastMultiplier = 1f;
    [SerializeField,Min(0)] private float MaxTilt = 15f;
    [SerializeField, Min(0)] private float smoothTime = 0.1f;
    float currentVelocity;

    void Update()
    {
        float newAngle = Mathf.SmoothDampAngle(transform.localEulerAngles.z, Mathf.Clamp(ballastScale.WeigthBalance * ballastMultiplier,-MaxTilt, MaxTilt), ref currentVelocity, smoothTime);
        transform.localRotation = Quaternion.Euler(new(transform.localEulerAngles.x,transform.localEulerAngles.y,newAngle));
    }
}