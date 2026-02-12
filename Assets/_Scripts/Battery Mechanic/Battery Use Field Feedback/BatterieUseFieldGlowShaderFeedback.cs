using DG.Tweening;
using UnityEngine;

public class BatterieUseFieldShaderFeedback : BatteryUseFieldFeedback
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialIndex = 1;
    [SerializeField] private string materialAttributeName = "_GlowStrength";

    private Material material;

    protected override void OnValidate()
    {
        if(!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
    }
    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
        material.SetFloat(materialAttributeName,0);
    }
    protected override void UpdateIsReady(bool isReady)
    {
        if(isReady)
            material.DOFloat(1,materialAttributeName,.25f);
        else
            material.DOFloat(0,materialAttributeName,.25f);

    }
}