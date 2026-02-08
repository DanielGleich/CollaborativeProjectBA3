using FishNet.Object;
using FishNet.Object.Synchronizing;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Sets the intensity of Motor Soundeffects accross the Network
/// </summary>
public class NetworkedMotorSoundFeedback : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private VehicleMovement vehicleMovement;
    [SerializeField] private EventReference motorSFX;

    [Header("Settings")]
    [SerializeField] private string paramterName;

    public readonly SyncVar<float> Intensity = new SyncVar<float>();
    private EventInstance eventInstance;

    void OnEnable()
    {
        eventInstance = RuntimeManager.CreateInstance(motorSFX);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);

        vehicleMovement.OnUpdateInputDirection += SetIntensity;
        SetIntensity(vehicleMovement.InputDirection);
        Intensity.OnChange += UpdateFMODVariable;

        eventInstance.start();
    }
    void OnDisable()
    {
        vehicleMovement.OnUpdateInputDirection -= SetIntensity;
        Intensity.OnChange -= UpdateFMODVariable;

        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
    private void UpdateFMODVariable(float prev, float next, bool asServer)
    {
        eventInstance.setParameterByName(paramterName, next);
    }
    public void SetIntensity(float intensity)
    {
        // Smotthing of values should be done in FMOD
        Intensity.Value = Mathf.Abs(intensity);
    }
}