using UnityEngine;

public class VehicleSteering : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform[] wheelRotationPoints;

    [Header("Settings")]
    [SerializeField, Min(0)] private Vector3 rotationAxis = Vector3.up;
    [SerializeField, Min(0)] private float rotationAmount = 45;
    [SerializeField, Min(0)] private float smoothTime = .25f;

    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private Vector3 currentVelocity;


    public void SetInputDirection(Vector2 inputDirection)
    {
        targetRotation = rotationAxis * inputDirection.x * rotationAmount;
    }
    void Update()
    {
        currentRotation = Vector3.SmoothDamp(currentRotation,targetRotation,ref currentVelocity,smoothTime);
        foreach(var t in wheelRotationPoints)
        {
            t.localEulerAngles = currentRotation;
        }
    }
}