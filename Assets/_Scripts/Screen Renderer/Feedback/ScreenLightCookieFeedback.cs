using UnityEngine;

/// <summary>
/// Displays results of a ScreenRenderer as a light cookie
/// </summary>
public class ScreenLightCookieFeedback : ScreenRendererFeedback
{
    [Header("References")]
    [SerializeField] private Light spotLight;
    [SerializeField] private Texture inactiveTexture;
    protected override void OnValidate()
    {
        base.OnValidate();
        if (!spotLight)
            spotLight = GetComponentInChildren<Light>();
    }
    protected override void UpdateTexture()
    {
        spotLight.cookie = screenRenderer.RenderTexture && screenRenderer.CameraDisplayActive ? 
            screenRenderer.RenderTexture : inactiveTexture;

    }
}