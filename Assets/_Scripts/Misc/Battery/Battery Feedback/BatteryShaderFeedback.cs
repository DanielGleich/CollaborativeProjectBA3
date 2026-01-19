using UnityEngine;

public class BatteryShaderFeedback : BatteryFeedback
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialIndex = 1;
    [SerializeField] private string materialAttributeName = "_GlowStrength";

    private Material material;

    void OnValidate()
    {
        if(!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
    }

    protected override void UpdateChargeAmountFeedback(float batteryCharging)
    {
        material.SetFloat(materialAttributeName, batteryCharging/ battery.MaxChargeVolume);
    }

    protected override void UpdateChargeCompletionFeedback(bool obj)
    {
        // Do nothing
    }
}