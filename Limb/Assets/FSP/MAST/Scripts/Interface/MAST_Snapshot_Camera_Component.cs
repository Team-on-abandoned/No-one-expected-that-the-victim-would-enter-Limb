using UnityEngine;

#if (UNITY_EDITOR)

[ExecuteInEditMode]
public class MAST_Snapshot_Camera_Component : MonoBehaviour
{
    
    const int prefabLayer = 2;
    
    [SerializeField] private Camera snapshotCamera;
    
    private GameObject subject;
    
    // ---------------------------------------------------------------------------
    // Get thumbnail array
    // ---------------------------------------------------------------------------
    public Texture2D[] GetThumbnails(GameObject[] prefabs)
    {
        // Set orientation for entire camera rig based on pitch and yaw
        gameObject.transform.eulerAngles = new Vector3(
            MAST_Settings.gui.palette.snapshotCameraYaw,
            MAST_Settings.gui.palette.snapshotCameraPitch,
            0f);
        
        // Initialize thumbnail array
        Texture2D[] thumbnails = new Texture2D[prefabs.Length];
        
        // Prep camera
        if (snapshotCamera == null)
            snapshotCamera = gameObject.GetComponentInChildren<Camera>();
        
        snapshotCamera.clearFlags = CameraClearFlags.SolidColor;
        snapshotCamera.backgroundColor = new Color(0f, 0f, 0f, 0.25f);
        
        snapshotCamera.cullingMask = 1 << prefabLayer;
        
        // Take a snapshot of each prefab and add to thumbnail array
        for (int i = 0; i < prefabs.Length; i++)
            thumbnails[i] = TakeSnapshot(prefabs[i]);
        
        // Return thumbnail array
        return thumbnails;
    }
    
    // ---------------------------------------------------------------------------
    // Instantiate gameobject and take a snapshot - returned as a Texture2D
    // ---------------------------------------------------------------------------
    private Texture2D TakeSnapshot(GameObject prefab)
    {
        // ----------------------------------
        // Instantiate prefab
        // ----------------------------------
        subject = GameObject.Instantiate(prefab);
        
        // Set prefab parent and child layers
        SetLayerRecursively(subject.GetComponent<Transform>(), prefabLayer);
        
        // ----------------------------------
        // Calculate Prefab Bounds
        // ----------------------------------
        Bounds bounds = new Bounds(new Vector3(0f, 0f, 0f), Vector3.zero);
        
        foreach (Renderer renderer in subject.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(renderer.bounds);
        
        // ----------------------------------
        // Fit Camera to GameObject
        // ----------------------------------
        float cameraDistance = 2.5f; // Constant factor
        Vector3 objectSizes = bounds.max - bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 3.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * snapshotCamera.fieldOfView); // Visible height 1 meter in front
        float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
        snapshotCamera.transform.position = bounds.center - distance * snapshotCamera.transform.forward;
        
        // ----------------------------------
        // Take snapshot of GameObject
        // ----------------------------------
        var renderTarget = RenderTexture.GetTemporary(128, 128);
        
        snapshotCamera.targetTexture = renderTarget;
        snapshotCamera.Render();
        
        RenderTexture.active = renderTarget;
        Texture2D snapshot = new Texture2D(renderTarget.width, renderTarget.height, TextureFormat.RGBA32, false);
        snapshot.ReadPixels(new Rect(0, 0, renderTarget.width, renderTarget.height), 0, 0);
        
        // Destroy instantiated prefab
        Object.DestroyImmediate(subject);
        
        snapshot.Apply();
        
        RenderTexture.active = null;
        snapshotCamera.targetTexture = null;
        
        return snapshot;
    }
    
    // ---------------------------------------------------------------------------
    // Recursively loop through all children and set their layers
    // ---------------------------------------------------------------------------
    private void SetLayerRecursively(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        
        foreach (Transform childTransform in transform)
            SetLayerRecursively(childTransform, layer);
    }
    
}

#endif