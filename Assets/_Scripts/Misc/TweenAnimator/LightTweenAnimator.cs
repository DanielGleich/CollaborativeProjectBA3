using DG.Tweening;
using UnityEngine;

public class LightTweenAnimator : TweenAnimator
{
    [Header("References")]
    [SerializeField] private Light attachedLight;

    [Header("Intensity")]
    [SerializeField, Min(0)] private float baseIntensity = 0;
    [SerializeField, Min(0)] private float activeIntensity = 1;

    [Header("Color")]
    [SerializeField] private bool animateColor = false;
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color activeColor = Color.white;

    void OnValidate()
    {
        if(!attachedLight)
            attachedLight = GetComponentInChildren<Light>();
    }

    void Awake()
    {
        attachedLight.intensity = !startActive ? baseIntensity : activeIntensity;
        if (animateColor)
            attachedLight.color = !startActive ? baseColor : activeColor;
    }

    public override void Activate()
    {
        attachedLight.DOIntensity(activeIntensity, tweenDuration).SetEase(ease);
        if (animateColor)
            attachedLight.DOColor(activeColor, tweenDuration).SetEase(ease);
    }

    public override void Deactivate()
    {
        attachedLight.DOIntensity(baseIntensity, tweenDuration).SetEase(ease);
        if (animateColor)
            attachedLight.DOColor(baseColor, tweenDuration).SetEase(ease);
    }
}