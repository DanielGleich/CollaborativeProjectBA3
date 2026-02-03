using DG.Tweening;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] private bool startActive = false;

    [Header("Shader")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialIndex;
    [SerializeField] private string propertyName = "_GlowStrength";
    [SerializeField] private Vector2 shaderRange = new(0, 1);

    [Header("Light")]
    [SerializeField] private Light attachedLight;
    [SerializeField, Min(0)] private Vector2 lightIntensityRange = new(0, 1);

    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = .5f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Material material;

    void OnValidate()
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
        if (!attachedLight)
            attachedLight = GetComponent<Light>();
    }

    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
        material.SetFloat(propertyName, !startActive ? shaderRange.x : shaderRange.y);
        attachedLight.intensity = !startActive ? lightIntensityRange.x : lightIntensityRange.y;
    }
    [ContextMenu("Activate")]
    public void Activate()
    {
        Debug.Log($"Activate {name}");
        attachedLight.DOIntensity(lightIntensityRange.y, tweenDuration).SetEase(ease);
        material.DOFloat(shaderRange.y, propertyName, tweenDuration).SetEase(ease);
    }
    [ContextMenu("Deactivate")]
    public void Deactivate()
    {
        Debug.Log($"Deactivate {name}");
        attachedLight.DOIntensity(lightIntensityRange.x, tweenDuration).SetEase(ease);
        material.DOFloat(shaderRange.x, propertyName, tweenDuration).SetEase(ease);
    }
    public void Activate(bool isActive)
    {
        if(isActive)
            Activate();
        else
            Deactivate();
    }
}