using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks total ballast and distribution of ballast inside of trigger zone
/// </summary>
public class BallastScale : MonoBehaviour {

    [Header("Settings")]
    [SerializeField, Tooltip("x = left/ right balance, y = forward/ backwards balance")] 
    private Vector2 ballastMultiplier = new(1,0);

    private Vector2 weightBalance;
    public Vector2 WeightBalance{
        get => weightBalance;
        private set
        {
            if(value == weightBalance)
                return;
            weightBalance = value;
            OnUpdateWeightBalance?.Invoke(value);
        }
    }
    private float totalWeight;
    public float TotalWeight
    {
        get => totalWeight;
        private set
        {
            if(value == totalWeight)
                return;
            totalWeight = value;
            OnUpdateTotalWeight?.Invoke(value);
        }
    }
    public event Action<Vector2> OnUpdateWeightBalance;
    public event Action<float> OnUpdateTotalWeight;

    private List<Ballast> ballastOnScale = new();

    void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Ballast>(out var ballast))
        {
            if(ballastOnScale.Contains(ballast))
                return;
            ballastOnScale.Add(ballast);
            ballast.OnDestroyBallast += RemoveDestoryedBallast;
            UpdateTotalWeight();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Ballast>(out var ballast))
        {
            ballastOnScale.Remove(ballast);
            ballast.OnDestroyBallast -= RemoveDestoryedBallast;
            UpdateTotalWeight();
        }
    }
    void Update()
    {
        if(ballastOnScale.Count < 1)
            return;
        UpdateWeightBalance();
    }
    void OnDisable()
    {
        ballastOnScale.ForEach(x => x.OnDestroyBallast -= RemoveDestoryedBallast);
    }
    private void UpdateTotalWeight()
    {
        float newTotalWeight = 0;
        ballastOnScale.ForEach(x => newTotalWeight += x.Weight);
        TotalWeight = newTotalWeight;
    }
    private void UpdateWeightBalance()
    {
        Vector2 newWeigthBlance = Vector2.zero;
        foreach(Ballast b in ballastOnScale)
        {
            Vector3 localDistance = transform.worldToLocalMatrix.MultiplyVector(transform.position - b.transform.position);
            newWeigthBlance.x += b.Weight * localDistance.x * ballastMultiplier.x;
            newWeigthBlance.y += b.Weight * -localDistance.z * ballastMultiplier.y;
        }
        WeightBalance = newWeigthBlance;
    }
    private void RemoveDestoryedBallast(Ballast ballast)
    {
        ballastOnScale.Remove(ballast);
    }
}