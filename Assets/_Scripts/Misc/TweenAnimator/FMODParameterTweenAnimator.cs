using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODParameterTweenAnimator : TweenAnimator
{
    [SerializeField] private EventReference eventReference;
    [SerializeField] private string paramterName;

    [Header("Settings")]
    [SerializeField, Tooltip("When set to false it will set the volume instead")] private bool setParameter = true;
    [SerializeField] private float baseValue = 0;
    [SerializeField] private float activeValue = 1;

    EventInstance eventInstance;
    private float currentValue;

    void OnEnable()
    {
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
        SetCurrentvalue(!startActive? baseValue: activeValue);
        eventInstance.start();
    }
    void OnDisable()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
    public override void Activate()
    {
        DOTween.To(x => SetCurrentvalue(x), currentValue, activeValue, tweenDuration).SetEase(ease);
    }
    public override void Deactivate()
    {
        DOTween.To(x => SetCurrentvalue(x), currentValue, baseValue, tweenDuration).SetEase(ease);
    }
    private void SetCurrentvalue(float x)
    {
        currentValue = x;
        if (setParameter)
            eventInstance.setParameterByName(paramterName, currentValue);
        else
            eventInstance.setVolume(currentValue);
    }
}