using DG.Tweening;
using UnityEngine;

/// <summary>
/// Animates a attribute with the type float of a shader and also lets you manualy set its values from the outside 
/// </summary>
public class MaterialAttributAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField, Min(0)] private int materialIndex = 0;
    [SerializeField] private string materialAttributeName = "_GlowStrength";

    [Header("Settings")]
    [SerializeField] private float baseValue = 0;
    [SerializeField] private float activeValue = 1;

    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = 2f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Material material;

    private void OnValidate()
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
    }
    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
        material.SetFloat(materialAttributeName, baseValue);
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        material.DOFloat(activeValue, materialAttributeName, tweenDuration).From(material.GetFloat(materialAttributeName)).SetEase(ease);
    }
    [ContextMenu("Deactivate")]
    public void Deactivate()
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