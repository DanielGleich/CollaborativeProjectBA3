using UnityEngine;

/// <summary>
/// Decides wich screen is activated & wich cameras output it should show
/// </summary>
public class ScreenRendererManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private VehilcleCamerasDummy vehilcleCameraDummy;
    [SerializeField, Tooltip("The order should according to the vehicle cameras class")] private ScreenRenderer[] screenRenderers;

    [Header("Settings")]
    [SerializeField] private bool singleActiveScreen;
    [SerializeField] private bool activateAllOnFound = true;

    private int activeIndex = -1;

    void OnEnable()
    {
        vehilcleCameraDummy.OnComponentFound += VehicleCamerasFound;
    }
    void OnDisable()
    {
        vehilcleCameraDummy.OnComponentFound -= VehicleCamerasFound;
    }
    private void VehicleCamerasFound()
    {
        SetUpScreenRenderCameras();
        if(activateAllOnFound)
            ActivateAllScreens();
    }
    public void ActivateSingleScreen(int index)
    {
        activeIndex = index;
        if(!vehilcleCameraDummy.vehicleCamera || activeIndex < 0)
            return;
        screenRenderers[index].RenderCamera = vehilcleCameraDummy.vehicleCamera.cameras[index];
        if(singleActiveScreen)
            DisableAllScreens();
        screenRenderers[index].StartRenderRoutine();
    }
    public void ActivateAllScreens()
    {
        Debug.Log("Activate all screens");
        foreach(ScreenRenderer s in screenRenderers)
            s.StartRenderRoutine();
    }
    private void SetUpScreenRenderCameras()
    {
        if(!vehilcleCameraDummy.vehicleCamera)
            return;
        if(screenRenderers.Length > vehilcleCameraDummy.vehicleCamera.cameras.Length)
        {
            Debug.LogError("Not enough vehicle cameras");
            return;
        }
        for (int i = 0; i < screenRenderers.Length; i++)
        {
            screenRenderers[i].RenderCamera = vehilcleCameraDummy.vehicleCamera.cameras[i];
        }
    }
    public void DisableAllScreens()
    {
        Debug.Log("Disable all screens");
        activeIndex = -1;
        foreach(ScreenRenderer s in screenRenderers)
            s.StopRenderRoutine();
    }
}