using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to keep track of how much weigth is on the ship and how it is distributed
/// </summary>
public class BallastScale : MonoBehaviour {
    [Header("Settings")]
    [SerializeField]
    private List<Ballast> ballastOnScale = new();
    public float WeigthBalance {get; private set;}
    public Action<float> OnUpdateTotalWeigth;
    public float TotalWeight {get; private set;}

    void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Ballast>(out var ballast))
        {
            if(ballastOnScale.Contains(ballast))
                return;
            ballastOnScale.Add(ballast);
            UpdateTotalWeigth();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Ballast>(out var ballast))
        {
            ballastOnScale.Remove(ballast);
            UpdateTotalWeigth();
        }
    }
    private void UpdateTotalWeigth()
    {
        float totalWeight = 0;
        ballastOnScale.ForEach(x => totalWeight += x.Weight);
        TotalWeight = totalWeight;
        OnUpdateTotalWeigth?.Invoke(totalWeight);
    }
    void Update()
    {
        UpdateWheigthBalance();
    }
    private void UpdateWheigthBalance()
    {
        float newBalance = 0;
        foreach(Ballast b in ballastOnScale)
        {
            if(b)
                newBalance += b.Weight * (transform.localPosition - b.transform.localPosition).x;
        }
        WeigthBalance = newBalance;
        // Debug.Log($"Weigth Balance = {WeigthBalance}, Total Weigth = {TotalWeight}");
    }
}