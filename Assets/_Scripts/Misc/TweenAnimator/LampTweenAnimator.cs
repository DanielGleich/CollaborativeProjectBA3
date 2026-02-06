using UnityEngine;

public class LampTweenAnimator : TweenAnimator
{
    [Header("References")]
    [SerializeField] private LightTweenAnimator lightTweenAnimator;
    [SerializeField] private MaterialTweenAnimator materialTweenAnimator;

    [Header("Settings")]
    [SerializeField] private bool overrideTweenSettings;

    void OnValidate()
    {
        // Searches for missing components and if needed adds the to the game object
        if(!lightTweenAnimator)
            lightTweenAnimator = GetComponentInChildren<LightTweenAnimator>();
        if(!lightTweenAnimator)
            lightTweenAnimator = gameObject.AddComponent<LightTweenAnimator>();
        if(!materialTweenAnimator)
            materialTweenAnimator = GetComponentInChildren<MaterialTweenAnimator>();
        if(!materialTweenAnimator)
            materialTweenAnimator = gameObject.AddComponent<MaterialTweenAnimator>();
        
        // Ovwerwrites Twenn Settings when active
        if(lightTweenAnimator && overrideTweenSettings)
            lightTweenAnimator.Init(tweenDuration, ease, startActive);
        if(materialTweenAnimator && overrideTweenSettings)
            materialTweenAnimator.Init(tweenDuration, ease, startActive);
    }

    public override void Activate()
    {
        lightTweenAnimator.Activate();
        materialTweenAnimator.Activate();
    }
    public override void Deactivate()
    {
        lightTweenAnimator.Deactivate();
        materialTweenAnimator.Deactivate();
    }
}