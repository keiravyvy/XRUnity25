using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MagnifyingGlass : MonoBehaviour
{
    [Header("Magnification Settings")]
    [SerializeField] private float magnificationFactor = 2f;
    [SerializeField] private LayerMask visibleLayers; // Regular environment
    [SerializeField] private LayerMask hiddenLayers;  // Hidden objects
    
    [Header("Lens Effects")]
    [SerializeField, Range(0, 5)] private float distortion = 1f;
    [SerializeField, Range(0, 1)] private float edgeBlur = 0.1f;
    [SerializeField] private Color glassColor = new Color(1, 1, 1, 0.1f);
    
    private Camera mainCamera;
    private Camera magnificationCamera;
    private RenderTexture renderTexture;
    private Material lensMaterial;
    private bool isDragging;
    private Vector3 lastMousePosition;

    void Start()
    {
        SetupComponents();
        SetupMagnificationCamera();
        SetupLensMaterial();
    }

    private void SetupComponents()
    {
        mainCamera = Camera.main;
        // Ensure main camera only sees regular environment
        //mainCamera.cullingMask = visibleLayers;
    }

    private void SetupMagnificationCamera()
    {
        // Create magnification camera that can see both regular and hidden objects
        GameObject magCamObj = new GameObject("MagnificationCamera");
        magnificationCamera = magCamObj.AddComponent<Camera>();
        magnificationCamera.enabled = true;
        magnificationCamera.clearFlags = CameraClearFlags.SolidColor;
        magnificationCamera.backgroundColor = Color.clear;
        magnificationCamera.cullingMask = visibleLayers | hiddenLayers;

        renderTexture = new RenderTexture(1024, 1024, 24);
        renderTexture.antiAliasing = 4;
        magnificationCamera.targetTexture = renderTexture;
    }

    private void SetupLensMaterial()
    {
        lensMaterial = GetComponent<Renderer>().material;
        lensMaterial.mainTexture = renderTexture;
        UpdateLensEffects();
    }

    private void UpdateLensEffects()
    {
        lensMaterial.SetFloat("_Distortion", distortion);
        lensMaterial.SetFloat("_EdgeBlur", edgeBlur);
        lensMaterial.SetColor("_GlassColor", glassColor);
    }

    void Update()
    {
        UpdateMagnificationCamera();
    }

    private void UpdateMagnificationCamera()
    {
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        float distanceToCamera = directionToCamera.magnitude;
        
        magnificationCamera.transform.position = transform.position - transform.forward * distanceToCamera;
        magnificationCamera.transform.LookAt(transform.position + transform.forward);
        magnificationCamera.fieldOfView = mainCamera.fieldOfView / magnificationFactor;
    }

    void OnDestroy()
    {
        if (renderTexture != null)
            renderTexture.Release();
        if (magnificationCamera != null)
            Destroy(magnificationCamera.gameObject);
    }
}