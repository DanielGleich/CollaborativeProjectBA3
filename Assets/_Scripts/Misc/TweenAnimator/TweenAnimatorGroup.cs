using System;
using UnityEngine;

public class TweenAnimatorGroup : TweenAnimator
{
    [Header("Refernces")]
    [SerializeField] private TweenAnimator[] tweenAnimators;

    [Header("Settings")]
    [SerializeField] private bool overrideTweenSettings;

    void OnValidate()
    {
        if(overrideTweenSettings)
            foreach(TweenAnimator t in tweenAnimators)
                if(t)
                    t.Init(tweenDuration, ease, startActive);
    }

    public override void Activate()
    {
        Array.ForEach(tweenAnimators, t => t.Activate());
    }
    public override void Deactivate()
    {
        Array.ForEach(tweenAnimators, t => t.Deactivate());
    }
}