using DG.Tweening;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Speed Settings")]
    [SerializeField] private float baseSpeed = 0;
    [SerializeField] private float activeSpeed = 1;

    [Header("Rotaion Settings")]
    [SerializeField] private Vector3 rotationAxis = new(0, 1, 0);
    [SerializeField] private bool useLocalRotation = true;

    [Header("Tweening Settings")]
    [SerializeField, Min(0)] private float tweenTime = .25f;
    [SerializeField] private Ease ease = Ease.Linear;

    private float currentSpeed;

    void OnValidate()
    {
        if (!target)
            target = gameObject.transform;
    }
    private void Awake()
    {
        currentSpeed = baseSpeed;
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
    public void Activate()
    {
        Debug.Log("Activate");
        DOTween.To(x => currentSpeed = x, currentSpeed, activeSpeed, tweenTime).SetEase(ease);
    }
    public void Deactivate()
    {
        Debug.Log("Deactivate");
        DOTween.To(x => currentSpeed = x, currentSpeed, baseSpeed, tweenTime).SetEase(ease);
    }
    public void Activate(bool activate)
    {
        if (activate)
            Activate();
        else
            Deactivate();
    }
}
