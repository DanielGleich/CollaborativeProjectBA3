using UnityEngine;

public class ScreenLightCookie : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Light spotLight;
    [SerializeField] private ScreenRenderer screenRenderer;
    [SerializeField] private Texture inactiveTexture;
    private void OnValidate() {
        
        if(!screenRenderer)
            screenRenderer = GetComponent<ScreenRenderer>();
        if(!spotLight)
            spotLight = GetComponentInChildren<Light>();
    }

    void OnEnable()
    {
        screenRenderer.OnUpdateTexture += UpdateTexture;
        screenRenderer.OnActivateCamera += ActivateCamera;
        UpdateTexture();
    }
    void OnDisable()
    {
        screenRenderer.OnUpdateTexture -= UpdateTexture;
        screenRenderer.OnActivateCamera -= ActivateCamera;
    }
    private void ActivateCamera(bool obj)
    {
        UpdateTexture();
    }
    private void UpdateTexture()
    {
        if(screenRenderer.RenderTexture && screenRenderer.CameraDisplayActive)
            spotLight.cookie = screenRenderer.RenderTexture;
        else
            spotLight.cookie = inactiveTexture;
    }
}