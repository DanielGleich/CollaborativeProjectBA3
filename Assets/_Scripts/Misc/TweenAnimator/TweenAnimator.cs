using DG.Tweening;
using UnityEngine;

public abstract class TweenAnimator : MonoBehaviour {
    [Header("Tween Settings")]
    [SerializeField] protected float tweenDuration = .5f;
    [SerializeField] protected Ease ease = Ease.InOutSine;

    [Header("Start Settings")]
    [SerializeField] protected bool startActive = false;

    public abstract void Activate();
    public abstract void Deactivate();
    public void Activate(bool activate)
    {
        if (activate)
            Activate();
        else
            Deactivate();
    }
    [ContextMenu("Trigger Activate")]
    private void DebugActivate() => Activate();

    [ContextMenu("Trigger Deactivate")]
    private void DebugDeactivate() => Deactivate();

    public void Init(float tweenDuration, Ease ease, bool startActive)
    {
        this.tweenDuration = tweenDuration;
        this.ease = ease;
        this.startActive = startActive;
    }
}