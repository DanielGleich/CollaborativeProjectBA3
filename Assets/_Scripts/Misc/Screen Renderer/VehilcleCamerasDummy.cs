using System;
using UnityEngine;

public class VehilcleCamerasDummy : NetworkedDummy
{
    [HideInInspector] public VehicleCameras vehicleCamera;
    public event Action OnComponentFound;
    public bool VehicleCamerasFound {get; private set;} = false;
    protected override void GetNetworkedComponent(GameObject other)
    {
        if(other.TryGetComponent<VehicleCameras>(out vehicleCamera))
        {
            Debug.Log("Vehicle Camera found");
            OnComponentFound?.Invoke();
            VehicleCamerasFound = true;
        }
        else
            Debug.LogError("Component not found");

    }
}