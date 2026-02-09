using FishNet.Object;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Global Ambience/ Music that only plays if you are the owner of this script and you have right team role (avoids hearing ambience special to certain roles at the same time)
/// </summary>
public class ClientLocalRoleBasedAudio : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private EventReference ambienceReference;
    [SerializeField] private TeamMember teamMember;

    [Header("Settings")]
    [SerializeField] private TeamRole teamRole;
    [SerializeField] private bool playOnStartClient = true;

    private EventInstance eventInstance;

    protected override void OnValidate()
    {
        base.OnValidate();
        if(!teamMember)
            GetComponentInParent<TeamMember>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner || teamMember.CurrentRole.Value != teamRole)
        {
            Destroy(this);
            return;
        }
        eventInstance = RuntimeManager.CreateInstance(ambienceReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
        if (playOnStartClient && gameObject.activeInHierarchy)
            Play();
    }
    public void Play() => eventInstance.start();
    public void Stop() => eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    public override void OnStopClient()
    {
        eventInstance.release();
        base.OnStopClient();
    }
}