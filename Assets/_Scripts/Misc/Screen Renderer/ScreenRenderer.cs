using System;
using System.Collections;
using UnityEngine;

public class ScreenRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera renderCamera;

    [Header("Material & Shader Settings")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialIndex;
    [SerializeField] private string screenVariableName = "_Screen";

    [Header("Render Settings")]
    [SerializeField] private Vector2Int imageResolution = new(480, 360);
    [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;
    [SerializeField] private RenderTextureFormat renderTextureFormat;
    [SerializeField, Tooltip("0 = every frame")] private float updateTime = 0f;

    [Header("Playback Settings")]
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private bool ignoreVisibility = true;

    [Header("Preview Settings")]
    [SerializeField, Tooltip("Show changes everytime you change a value")] private bool updatePreviewOnValidate;
    private RenderTexture renderTexture;
    private Material material;

    public Camera RenderCamera
    {
        get => renderCamera;
        set
        {
            if(value == renderCamera)
                return;
            renderCamera = value;
            OnSwitchRenderCamera?.Invoke(value);
            
            if(cameraDisplayActive)
                StartRenderRoutine();
        }
    }
    public event Action<Camera> OnSwitchRenderCamera;

    private bool cameraDisplayActive;
    public event Action<bool> OnActivateCamera;
    public bool CameraDisplayActive
    {
        get => cameraDisplayActive;
        protected set
        {
            if(value == cameraDisplayActive)
                return;
            cameraDisplayActive = value;
            OnActivateCamera?.Invoke(value);
        }
    }

    void OnValidate()
    {
        if(!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
        if (updatePreviewOnValidate)
            StartCoroutine(PreviewRenderTextureRoutine());
    }

    void Awake()
    {
        material = meshRenderer.materials[materialIndex];
    }

    void OnEnable()
    {
        if (playOnEnable)
            StartRenderRoutine();
    }
    void OnDisable() => StopRenderRoutine();

    public void StartRenderRoutine()
    {
        if (!renderCamera)
            return;
        StopAllCoroutines();
        StartCoroutine(RenderRoutine());
        CameraDisplayActive = true;
    }
    public void StopRenderRoutine()
    {
        Debug.Log("Stop Render Routine");
        StopAllCoroutines();
        OnActivateCamera?.Invoke(false);
    }
    private IEnumerator RenderRoutine()
    {
        Debug.Log("Start Render Routine");
        CreateRenderTexture();
        yield return null;
        while (true)
        {
            Render();
            yield return new WaitForSeconds(updateTime);
        }
    }
    public bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
    private void CreateRenderTexture(bool useSharedMaterial = false)
    {
        renderTexture = new RenderTexture(imageResolution.x, imageResolution.y, 1000, renderTextureFormat);
        renderTexture.filterMode = filterMode;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderCamera.targetTexture = renderTexture;
        if (useSharedMaterial)
            meshRenderer.sharedMaterials[materialIndex].SetTexture(screenVariableName, renderTexture);
        else
            material.SetTexture(screenVariableName, renderTexture);
    }
    private void Render()
    {
        if  (ignoreVisibility || VisibleFromCamera(meshRenderer, Camera.main) ||Application.isEditor)
            renderCamera.Render();
    }

    #region Preview
    [ContextMenu("Preview Render Texture")]
    private void PreviewRenderTexture() => StartCoroutine(PreviewRenderTextureRoutine());

    private IEnumerator PreviewRenderTextureRoutine()
    {
        if (!(renderCamera && meshRenderer))
            yield break;
        yield return null;
        CreateRenderTexture(true);
        Render();
    }
    #endregion
}