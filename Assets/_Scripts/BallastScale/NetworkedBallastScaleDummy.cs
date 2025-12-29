using System;
using UnityEngine;

public class NetworkedBallastScaleDummy : NetworkedDummy
{
    private BallastScaleNetworking ballastScaleNetworking;
    public Vector2 WeightBalance => ballastScaleNetworking? ballastScaleNetworking.WeightBalance.Value : Vector2.zero;
    public event Action<Vector2> OnUpdateWeightBalance;
    protected override void GetNetworkedComponent(GameObject other)
    {
        ballastScaleNetworking = other.GetComponent<BallastScaleNetworking>();
        if(ballastScaleNetworking)
            ballastScaleNetworking.WeightBalance.OnChange += UpdateWeightBalance;
        else
            Debug.LogError("Component not found");
    }
    private void UpdateWeightBalance(Vector2 prev, Vector2 next, bool asServer)
    {
        Debug.Log("Updated Weigth Balance: " + next);
        OnUpdateWeightBalance?.Invoke(next);
    }
    void OnDestroy()
    {
        if(ballastScaleNetworking)
            ballastScaleNetworking.WeightBalance.OnChange -= UpdateWeightBalance;
    }
}