using UnityEngine;

/// <summary>
/// Displays results of a screen renderer on a texture
/// </summary>
public class ScreenMaterialFeedback : ScreenRendererFeedback
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialIndex = 1;
    [SerializeField] private string screenVariableName = "_Screen";
    [SerializeField] private Texture inactiveTexture;

    private Material material;

    protected override void OnValidate()
    {
        base.OnValidate();
        if(!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
    }
    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
    }
    protected override void UpdateTexture()
    {
        material.SetTexture(screenVariableName, screenRenderer.RenderTexture && screenRenderer.CameraDisplayActive?
             screenRenderer.RenderTexture : inactiveTexture);
    }
}