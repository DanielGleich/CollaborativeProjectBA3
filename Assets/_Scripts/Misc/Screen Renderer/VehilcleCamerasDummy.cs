using System;
using UnityEngine;

public class VehilcleCamerasDummy : NetworkedDummy
{
    [HideInInspector] public VehicleCameras vehicleCamera;
    public event Action OnComponentFound;
    protected override void GetNetworkedComponent(GameObject other)
    {
        if(other.TryGetComponent<VehicleCameras>(out vehicleCamera))
        {
            Debug.Log("Vehicle Camera found");
            OnComponentFound?.Invoke();
        }
        else
            Debug.LogError("Component not found");

    }
}