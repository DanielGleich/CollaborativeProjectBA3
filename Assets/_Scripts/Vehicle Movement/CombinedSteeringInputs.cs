using System;
using UnityEngine;

/// <summary>
/// Combines scientist inputs & ballast scale inputs based on a influence value
/// </summary>
public class CombinedSteeringInputs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkedBallastScaleDummy ballastScale;
    [SerializeField] private VehicleSteering vehicleSteering;

    [Header("Settings")]
    [SerializeField] private float weigthBalanceMultiplier = -1;
    [SerializeField, Range(0, 1)] private float ballastScaleInfluence = .25f;
    [SerializeField, Min(0.001f), Tooltip("The amount of weight needed for the maximum steering power")] private float maxSteeringPowerWeight = 10f;

    public float WeightBalance { get; private set; }
    public float InputDirection { get; private set; }

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
        WeightBalance = vector.x;
        UpdateSteering();
    }
    public void SetInputDirection(Vector2 inputDirection)
    {
        this.InputDirection = Mathf.Round(inputDirection.x);
        UpdateSteering();
    }
    public void UpdateSteering()
    {
        float steeringInputX = InputDirection * (1 - ballastScaleInfluence) 
            + Mathf.Clamp(WeightBalance * weigthBalanceMultiplier / maxSteeringPowerWeight, -1, 1) * ballastScaleInfluence;
        vehicleSteering.SetInputDirection(new(steeringInputX, 0));
    }
}