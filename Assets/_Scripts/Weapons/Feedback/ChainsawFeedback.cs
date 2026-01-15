using DG.Tweening;
using UnityEngine;

public class ChainsawFeedback : WeaponFeedback
{
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Settings")]
    [SerializeField] private int materialIndex;
    [SerializeField] private string materialAttributeName = "_Scroll_Speed";

    [Header("Settings")]
    [SerializeField] private float baseSawSpeed = 0;
    [SerializeField] private float activeSawSpeed;

    [Header("Tween Settings")]
    [SerializeField] private float duration = .1f;
    [SerializeField] private Ease ease = Ease.InOutQuad;
    private Material material;

    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
        material.SetFloat(materialAttributeName, baseSawSpeed);
    }

    protected override void WeaponActivated(bool isActivated)
    {
        if (isActivated)
        {
            material.DOFloat(activeSawSpeed, materialAttributeName, duration).From(baseSawSpeed).SetEase(ease);
        }
        else
        {
            material.DOFloat(baseSawSpeed, materialAttributeName, duration).From(activeSawSpeed).SetEase(ease);
        }
    }
}