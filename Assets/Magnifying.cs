using UnityEngine;

public class Magnifying : MonoBehaviour
{
    [Header("Magnification Settings")]
    [SerializeField] private float magnificationFactor = 2f;
    [SerializeField] private LayerMask magnifiableLayers;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    private Camera magnificationCamera;
    private RenderTexture renderTexture;

    void Start()
    {
        // Get main camera if not assigned
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Create magnification camera
        GameObject magnificationCameraObj = new GameObject("MagnificationCamera");
        magnificationCamera = magnificationCameraObj.AddComponent<Camera>();
        magnificationCamera.enabled = true;
        magnificationCamera.clearFlags = CameraClearFlags.SolidColor;
        magnificationCamera.backgroundColor = Color.clear;
        magnificationCamera.cullingMask = magnifiableLayers;

        // Create render texture
        renderTexture = new RenderTexture(512, 512, 24);
        magnificationCamera.targetTexture = renderTexture;

        // Set material's texture
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.mainTexture = renderTexture;
        }
    }

    void LateUpdate()
    {
        // Update magnification camera position
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        float distanceToCamera = directionToCamera.magnitude;
        
        // Position the magnification camera behind the glass
        magnificationCamera.transform.position = transform.position - transform.forward * distanceToCamera;
        magnificationCamera.transform.LookAt(transform.position + transform.forward);

        // Match main camera's properties but with magnification
        magnificationCamera.fieldOfView = mainCamera.fieldOfView / magnificationFactor;
        magnificationCamera.nearClipPlane = mainCamera.nearClipPlane;
        magnificationCamera.farClipPlane = mainCamera.farClipPlane;
    }

    void OnDestroy()
    {
        // Clean up resources
        if (renderTexture != null)
            renderTexture.Release();
            
        if (magnificationCamera != null)
            Destroy(magnificationCamera.gameObject);
    }
}
