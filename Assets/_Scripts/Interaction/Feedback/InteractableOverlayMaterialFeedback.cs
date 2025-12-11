using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class InteractableOverlayMaterialFeedback : MonoBehaviour
{
    [Header("Refernces")]
    [SerializeField] private Interactable interactable;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Material Settings")]
    [SerializeField] private Material overlayMaterialReference;
    [SerializeField] private string effectName = "_Overlay_Alpha";

    [Header("Feedback Settings")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Material material;

    void OnValidate()
    {
        if (interactable == null)
            interactable = GetComponent<Interactable>();
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
        interactable.OnSelected += InteractableSelected;
    }
    void OnDisable()
    {
        interactable.OnSelected -= InteractableSelected;
    }
    public void InteractableSelected(bool isSelected)
    {
        if (isSelected)
        {
            material.DOFloat(1, effectName, duration).SetEase(ease);
        }
        else
        {
            material.DOFloat(0, effectName, duration).SetEase(ease);
        }
    }
}