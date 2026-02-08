using System;
using UnityEngine;

public class VehilcleCamerasDummy : NetworkedDummy
{
    public VehicleCameras VehicleCameras{get; private set;}
    public event Action OnComponentFound;
    
    protected override void GetNetworkedComponent(GameObject other)
    {
        VehicleCameras = other.GetComponent<VehicleCameras>();
        if(VehicleCameras)
        {
            Debug.Log("Vehicle Cameras found");
            OnComponentFound?.Invoke();
        }
        else
            Debug.LogError("Vehicle Cameras not found");

    }
}