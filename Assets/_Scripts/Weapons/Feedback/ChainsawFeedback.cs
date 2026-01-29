using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
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

    [Header("Audio")]
    [SerializeField] private EventReference ChainsawTriggeredSFX;

    private EventInstance chainSawEventInstance;
    private Material material;

    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
        material.SetFloat(materialAttributeName, baseSawSpeed);
        chainSawEventInstance = RuntimeManager.CreateInstance(ChainsawTriggeredSFX);
        RuntimeManager.AttachInstanceToGameObject(chainSawEventInstance, gameObject);
    }
    void OnDestroy()
    {
        chainSawEventInstance.release();
    }

    protected override void WeaponActivated(bool isActivated)
    {
        if (isActivated)
        {
            chainSawEventInstance.start();
            material.DOFloat(activeSawSpeed, materialAttributeName, duration).From(baseSawSpeed).SetEase(ease);
        }
        else
        {
            chainSawEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            material.DOFloat(baseSawSpeed, materialAttributeName, duration).From(activeSawSpeed).SetEase(ease);
        }
    }
}