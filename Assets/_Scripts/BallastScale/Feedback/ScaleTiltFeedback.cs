using UnityEngine;

/// <summary>
/// Tilts a game object based on the ballast scale (local rotation)
/// </summary>
public class ScaleTiltFeedback : MonoBehaviour {
    [Header("References")]
    [SerializeField] private BallastScale ballastScale;

    [Header("Settings")]
    [SerializeField, Min(0)] private Vector2 tiltMultiplier = new (1,1);
    [SerializeField] private Vector2 maxTilt = new (30, 30);
    [SerializeField, Min(0)] private float smoothTime = 0.1f;
    Vector2 targetTilt;
    float currentXAngleVelocity;
    float currentZAngleVelocity;

    void OnEnable()
    {
        ballastScale.OnUpdateWeightBalance += UpdateTargetBalance;
        UpdateTargetBalance(ballastScale.WeightBalance);
    }
    void OnDisable()
    {
        ballastScale.OnUpdateWeightBalance -= UpdateTargetBalance;
    }

    private void UpdateTargetBalance(Vector2 weightBalance)
    {
        targetTilt = weightBalance * tiltMultiplier;
    }

    void Update()
    {
        if(transform.localEulerAngles.z == targetTilt.x && transform.localEulerAngles.x == targetTilt.y)
            return;
        float z = Mathf.SmoothDampAngle(transform.localEulerAngles.z, Mathf.Clamp(ballastScale.WeightBalance.x * tiltMultiplier.x,-maxTilt.x, maxTilt.x), ref currentZAngleVelocity, smoothTime);
        float x = Mathf.SmoothDampAngle(transform.localEulerAngles.x, Mathf.Clamp(ballastScale.WeightBalance.y * tiltMultiplier.y,-maxTilt.y, maxTilt.y), ref currentXAngleVelocity, smoothTime);
        transform.localRotation = Quaternion.Euler(x,transform.localEulerAngles.y,z);
    }
}