using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class BallastScaleNetworking : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BallastScale ballastScale;
    public readonly SyncVar<Vector2> WeightBalance = new SyncVar<Vector2>();

    void OnEnable()
    {
        if(ballastScale)
            ballastScale.OnUpdateWeightBalance += UpdateWeightBalance;
    }

    void OnDisable()
    {
        if(ballastScale)
            ballastScale.OnUpdateWeightBalance -= UpdateWeightBalance;
    }
    private void UpdateWeightBalance(Vector2 weightBalance)
    {
        WeightBalance.Value = weightBalance;
    }
}