using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ConstantDamageFeedback : MonoBehaviour {

    [Header("References")]
    [SerializeField] private ConstantDamage constantDamage;

    [Header("Audio Feedback")]
    [SerializeField] private EventReference hitSFX;
    [SerializeField] private EventReference stayDamageSFX;

    private EventInstance eventInstance;

    void OnValidate()
    {
        if(!constantDamage)
            constantDamage = GetComponentInChildren<ConstantDamage>();
    }
    void OnEnable()
    {
        eventInstance = RuntimeManager.CreateInstance(stayDamageSFX);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, constantDamage.gameObject);
        constantDamage.OnHit += HitFeedback;
        constantDamage.OnUpdatedAffectHealthComponents += UpdatedAffectedHealthComponents;
        UpdatedAffectedHealthComponents(constantDamage.AffectedHealthComponentsCount);
    }
    void OnDisable()
    {
        constantDamage.OnHit -= HitFeedback;
        constantDamage.OnUpdatedAffectHealthComponents -= UpdatedAffectedHealthComponents;
        eventInstance.release();
    }
    private void HitFeedback()
    {
        RuntimeManager.PlayOneShot(hitSFX,constantDamage.transform.position);
    }
    private void UpdatedAffectedHealthComponents(int number)
    {
        // Nice to have for later would be to instead set a variable in FMOD
        if(number <= 0)
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        else
            eventInstance.start();       
    }
}