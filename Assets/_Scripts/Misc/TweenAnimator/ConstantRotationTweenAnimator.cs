using DG.Tweening;
using UnityEngine;

/// <summary>
/// Blends between multiple rotation speeds
/// </summary>
public class ConstantRotationTweenAnimator : TweenAnimator
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Speed Settings (1 = 1 hole rotation per second)")]
    [SerializeField] private float baseSpeed = 0;
    [SerializeField] private float activeSpeed = 1;

    [Header("Rotaion Settings")]
    [SerializeField] private Vector3 rotationAxis = new(0, 1, 0);
    [SerializeField] private bool useLocalRotation = true;

    private float currentSpeed;

    void OnValidate()
    {
        if (!target)
            target = gameObject.transform;
    }
    private void Awake()
    {
        currentSpeed = startActive? activeSpeed : baseSpeed;
        rotationAxis = rotationAxis.normalized;
    }
    void Update()
    {
        if (currentSpeed == 0)
            return;
        if (useLocalRotation)
            target.RotateAround(target.position, target.rotation * rotationAxis, currentSpeed * 360 * Time.deltaTime);
        else
            target.RotateAround(target.position, rotationAxis, currentSpeed * 360 * Time.deltaTime);
    }
    public override void Activate()
    {
        DOTween.To(x => currentSpeed = x, currentSpeed, activeSpeed, tweenDuration).SetEase(ease);
    }
    public override void Deactivate()
    {
        DOTween.To(x => currentSpeed = x, currentSpeed, baseSpeed, tweenDuration).SetEase(ease);
    }
}
