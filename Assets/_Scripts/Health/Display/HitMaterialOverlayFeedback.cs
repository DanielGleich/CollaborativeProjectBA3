using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using DG.Tweening;
using UnityEngine;

public class HitMaterialOverlayFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Material Settings")]
    [SerializeField] private Material overlayMaterialReference;
    [SerializeField] private string effectName = "_Overlay_Alpha";

    [Header("Feedback Settings")]
    [SerializeField] private float duration = 0.1f;
    // [SerializeField] private Ease ease = Ease.InOutSine;

    private Material material;

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
        health.OnDamaged += TriggerHitFeedback;
    }
    void OnDisable()
    {
        health.OnDamaged -= TriggerHitFeedback;
    }

    private void TriggerHitFeedback(Damage damage)
    {
        // material.DOFloat(0, effectName, duration).ChangeStartValue(1).SetEase(ease);
        StartCoroutine(HitFeedbackRoutine());
    }
    public IEnumerator HitFeedbackRoutine()
    {
        material.SetFloat(effectName,1);
        yield return new WaitForSeconds(duration);
        material.SetFloat(effectName,0);
    }
}
