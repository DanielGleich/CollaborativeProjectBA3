using System;
using UnityEngine;

public class VehicleSteeringControls : MonoBehaviour {
    [Header("References")]
    [SerializeField] private NetworkedBallastScaleDummy ballastScale;
    [SerializeField] private VehicleSteering vehicleSteering;

    [Header ("Settings")]
    [SerializeField] private float weigthBalanceMultiplier = -1;
    [SerializeField, Range(0,1)] private float ballastScaleInfluence = .25f;
    [SerializeField, Min(0.001f), Tooltip("The amount of weight needed for the maximum steering power")] private float maxSteeringPowerWeight = 10f;

    private float weightBalance;
    private float inputDirection;

    void OnEnable()
    {
        ballastScale.OnUpdateWeightBalance += UpdateWeightBalance;
    }
    void OnDisable()
    {
        ballastScale.OnUpdateWeightBalance -= UpdateWeightBalance;
    }
    private void UpdateWeightBalance(Vector2 vector)
    {
        weightBalance = vector.x;
        UpdateSteering();
    }
    public void SetInputDirection(Vector2 inputDirection)
    {
        this.inputDirection = Mathf.Round(inputDirection.x);
        UpdateSteering();
    }
    public void UpdateSteering()
    {
        float steeringInputX = inputDirection * (1 - ballastScaleInfluence) + Mathf.Clamp(weightBalance * weigthBalanceMultiplier / maxSteeringPowerWeight,-1,1) * ballastScaleInfluence;
        vehicleSteering.SetInputDirection(new(steeringInputX,0));
    }
}