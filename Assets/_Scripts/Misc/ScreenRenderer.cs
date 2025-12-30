using System.Collections;
using UnityEngine;

public class ScreenRenderer : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Camera renderCamera;

    [Header("Material & Shader Settings")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private string screenVariableName = "_Screen";

    [Header("Render Settings")]
    [SerializeField] private Vector2Int imageResolution = new (480, 360);
    [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;
    [SerializeField] private RenderTextureFormat renderTextureFormat;
    [SerializeField, Tooltip("0 = every frame")] private float updateRate = 0f;

    [Header("Preview")]
    [SerializeField, Tooltip("Show changes everytime you change a value")] private bool updatePreviewOnValidate;
    private RenderTexture renderTexture;

    void Start() => renderCamera.enabled = false;
    void OnEnable() => StartCoroutine(RenderRoutine());
    void OnDisable() => StopAllCoroutines();
    private IEnumerator RenderRoutine()
    {
        CreateRenderTexture();
        yield return null;
        while(true)
        {
            Render();
            yield return new WaitForSeconds(updateRate);
        }
    }
    void OnValidate()
    {
        if(updatePreviewOnValidate)
            StartCoroutine(PreviewRenderTextureRoutine());
    }

    public bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
    private void CreateRenderTexture()
    {
        renderTexture = new RenderTexture(imageResolution.x, imageResolution.y, 1000, renderTextureFormat);
        renderTexture.filterMode = filterMode;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderCamera.targetTexture = renderTexture;
        Debug.Log(Application.isEditor);
        if(Application.isEditor)
            meshRenderer.sharedMaterial.SetTexture(screenVariableName, renderTexture);
        else
            meshRenderer.material.SetTexture(screenVariableName, renderTexture);
    }
    private void Render()
    {
        if(VisibleFromCamera(meshRenderer, Camera.main) || Application.isEditor)
            renderCamera.Render();
    }

    #region Preview
    [ContextMenu("Preview Render Texture")]
    private void PreviewRenderTexture() => StartCoroutine(PreviewRenderTextureRoutine());

    private IEnumerator PreviewRenderTextureRoutine()
    {
        if(!(renderCamera && meshRenderer))
            yield break;
        yield return null;
        CreateRenderTexture();
        Render();
    }
    #endregion
}