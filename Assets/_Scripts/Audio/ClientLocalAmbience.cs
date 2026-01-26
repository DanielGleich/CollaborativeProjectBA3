using FishNet.Object;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Ambience that only plays if you are the owner of this script (avoids hearing ambience special to certain roles at the same time)
/// </summary>
public class ClientLocalAmbience : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private EventReference ambienceReference;

    [Header("Settings")]
    [SerializeField] private bool playOnStartClient = true;

    private EventInstance eventInstance;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!IsOwner)
        {
            Destroy(this);
            return;
        }
        eventInstance = RuntimeManager.CreateInstance(ambienceReference);
    }
    public void Play() => eventInstance.start();
    public void Stop() => eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    public override void OnStopClient()
    {
        base.OnStopClient();
        eventInstance.release();
    }
}