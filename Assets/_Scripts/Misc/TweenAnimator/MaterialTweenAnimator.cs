using DG.Tweening;
using UnityEngine;

/// <summary>
/// Animates a attribute with the type float of a material and also lets you manualy set its values from the outside 
/// </summary>
public class MaterialTweenAnimator : TweenAnimator
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField, Min(0)] private int materialIndex = 0;
    [SerializeField] private string materialAttributeName = "_GlowStrength";

    [Header("Settings")]
    [SerializeField] private float baseValue = 0;
    [SerializeField] private float activeValue = 1;

    private Material material;

    private void OnValidate()
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
    }
    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
        material.SetFloat(materialAttributeName, !startActive? baseValue: activeValue);
    }
    public override void Activate()
    {
        material.DOFloat(activeValue, materialAttributeName, tweenDuration).From(material.GetFloat(materialAttributeName)).SetEase(ease);
    }
    public override void Deactivate()
    {
        material.DOFloat(baseValue, materialAttributeName, tweenDuration).From(material.GetFloat(materialAttributeName)).SetEase(ease);
    }
    [ContextMenu("Flash")]
    public void Flash()
    {
        material.DOFloat(activeValue, materialAttributeName, tweenDuration).From(material.GetFloat(materialAttributeName)).SetEase(ease).OnComplete(Deactivate);
    }

    public void SetValue(float value) => material.SetFloat(materialAttributeName, value);
    public void ResetValue(float value) => material.SetFloat(materialAttributeName, baseValue);
}