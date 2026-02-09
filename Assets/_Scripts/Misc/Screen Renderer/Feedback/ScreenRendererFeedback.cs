using UnityEngine;

public abstract class ScreenRendererFeedback : MonoBehaviour {
    [Header("References")]
    [SerializeField] protected ScreenRenderer screenRenderer;

    protected virtual void OnValidate()
    {
        if(!screenRenderer)
            screenRenderer = GetComponent<ScreenRenderer>();
    }
    protected virtual void OnEnable()
    {
        screenRenderer.OnUpdateTexture += UpdateTexture;
        screenRenderer.OnActivateCamera += ActivateCamera;
        UpdateTexture();
    }
    protected virtual void OnDisable()
    {
        screenRenderer.OnUpdateTexture -= UpdateTexture;
        screenRenderer.OnActivateCamera -= ActivateCamera;
    }
    protected abstract void UpdateTexture();
    protected virtual void ActivateCamera(bool isActive) => UpdateTexture();
}