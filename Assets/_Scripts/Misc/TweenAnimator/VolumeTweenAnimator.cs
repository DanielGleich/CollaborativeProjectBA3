using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeTweenAnimator : TweenAnimator
{
    [Header("References")]
    [SerializeField] private Volume volume;

    [Header("Settings")]
    [SerializeField, Min(0)] private float baseWeight = 0;
    [SerializeField, Min(0)] private float activeWeight = 1;

    void OnEnable()
    {
        volume.weight = startActive? baseWeight : activeWeight;
    }

    public override void Activate()
    {
        DOTween.To(x => volume.weight = x, volume.weight, activeWeight, tweenDuration).SetEase(ease);
    }

    public override void Deactivate()
    {
        DOTween.To(x => volume.weight = x, volume.weight, baseWeight, tweenDuration).SetEase(ease);
    }
}
