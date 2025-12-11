using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class HitOverlayFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Material Settings")]
    [SerializeField] private Material overlayMaterialReference;
    [SerializeField] private string effectName = "_Overlay_Alpha";

    [Header("Feedback Settings")]
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Material material;
    private float prevoiusHealth;

    void OnValidate()
    {
        if (!health)
            health = GetComponent<Health>();
        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        List<Material> materials = meshRenderer.materials.ToList();
        materials.Add(overlayMaterialReference);
        meshRenderer.SetMaterials(materials);
        material = meshRenderer.materials[^1];
        material.SetFloat(effectName, 0);
    }
    void OnEnable()
    {
        health.OnUpdateHealth += TriggerHitFeedback;
        prevoiusHealth = health.CurrentHealth;
    }
    void OnDisable()
    {
        health.OnUpdateHealth-= TriggerHitFeedback;
    }

    private void TriggerHitFeedback(float health)
    {
        if(health < prevoiusHealth)
            material.DOFloat(0, effectName, duration).ChangeStartValue(1).SetEase(ease);
        prevoiusHealth = health;
    }
}
