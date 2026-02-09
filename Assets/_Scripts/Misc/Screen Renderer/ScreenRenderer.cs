using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manually renders cameras with adjustable resolution, filter mode and (ideal) framrate and displays it on a material
/// </summary>
public class ScreenRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera renderCamera;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Render Settings")]
    [SerializeField] private Vector2Int imageResolution = new(480, 360);
    [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;
    [SerializeField] private RenderTextureFormat renderTextureFormat;
    [SerializeField, Tooltip("0 = every frame")] private float updateTime = 0f;

    [Header("Playback Settings")]
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private bool ignoreVisibility = false;

    public event Action OnUpdateTexture;
    public RenderTexture RenderTexture {get; private set;}

    public event Action<Camera> OnSwitchRenderCamera;
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
        RenderTexture = new RenderTexture(imageResolution.x, imageResolution.y, 1000, renderTextureFormat);
        RenderTexture.filterMode = filterMode;
        RenderTexture.wrapMode = TextureWrapMode.Clamp;
        renderCamera.targetTexture = RenderTexture;
        OnUpdateTexture?.Invoke();
    }
    private void Render()
    {
        if  (ignoreVisibility || VisibleFromCamera(meshRenderer, Camera.main) ||Application.isEditor)
            renderCamera.Render();
    }
}