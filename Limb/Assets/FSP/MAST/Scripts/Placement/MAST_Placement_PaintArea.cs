using UnityEngine;

#if (UNITY_EDITOR)

public static class MAST_Placement_PaintArea
{
    [SerializeField] private static bool paintingArea = false;
    [SerializeField] private static Vector3 paintAreaStart = new Vector3(0f, 0f, 0f);
    [SerializeField] private static GameObject paintAreaVisualizer;
    [SerializeField] private static Material paintAreaMaterial;
    
    // Start paint area
    public static void StartPaintArea()
    {
        if (MAST_Placement_Visualizer.GetGameObject() != null)
        {
            // Set painting area to true
            paintingArea = true;
            
            // Record paint area start location
            paintAreaStart = MAST_Placement_Visualizer.GetGameObject().transform.position;
            paintAreaStart.y = MAST_Settings.gui.grid.gridHeight *
                MAST_Settings.gui.grid.yUnitSize + MAST_Const.grid.yOffsetToAvoidTearing;
            
            // Create new Paint Area Visualizer
            paintAreaVisualizer = GameObject.CreatePrimitive(PrimitiveType.Plane);
            paintAreaVisualizer.transform.position = new Vector3(0f, 0f, 0f);
            paintAreaVisualizer.name = "MAST_Paint_Area_Visualizer";
            
            // Configure Paint Area Visualizer MeshRenderer
            MeshRenderer paintAreaMeshRenderer = paintAreaVisualizer.GetComponent<MeshRenderer>();
            paintAreaMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            paintAreaMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            paintAreaMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            paintAreaMeshRenderer.receiveShadows = false;
            
            // Configure Paint Area Visualizer Material
            if (paintAreaMaterial == null)
                paintAreaMaterial = MAST_Asset_Loader.GetPaintAreaMaterial();
            paintAreaMeshRenderer.material = paintAreaMaterial;
            
            // Hide the Paint Area Visualizer in the hierarchy
            paintAreaVisualizer.hideFlags = HideFlags.HideInHierarchy;
            
            // Update the paint area
            UpdatePaintArea();
        }
    }
    
    // Update paint area
    public static void UpdatePaintArea()
    {
        // If painting area
        if (paintingArea)
        {
            // Get current mouse position on grid
            Vector3 paintAreaEnd = MAST_Placement_Helper.GetPositionOnGridClosestToMousePointer();
            paintAreaEnd.y = MAST_Settings.gui.grid.gridHeight *
                MAST_Settings.gui.grid.yUnitSize + MAST_Const.grid.yOffsetToAvoidTearing;
            
            // Make sure paint area start is at the current grid height, incase the grid was moved
            paintAreaStart.y = MAST_Settings.gui.grid.gridHeight *
                MAST_Settings.gui.grid.yUnitSize + MAST_Const.grid.yOffsetToAvoidTearing;
            
            // Get dimensions of paint area
            Vector3 scale = new Vector3(
                Mathf.Abs((paintAreaStart.x - paintAreaEnd.x) / 10f) + (MAST_Settings.gui.grid.xzUnitSize / 10),
                1,
                Mathf.Abs((paintAreaStart.z - paintAreaEnd.z) / 10f) + (MAST_Settings.gui.grid.xzUnitSize / 10));
            
            // Update paint area visualizer position to be between the start and end points
            paintAreaVisualizer.transform.position = (paintAreaStart + paintAreaEnd) / 2;
            
            // Update paint area visualizer x and z scale
            paintAreaVisualizer.transform.localScale = scale;
            
        }
    }
    
    // Complete paint area
    public static void CompletePaintArea()
    {
        // Get current mouse position on grid
        Vector3 paintAreaEnd = MAST_Placement_Helper.GetPositionOnGridClosestToMousePointer();
        paintAreaEnd.y = MAST_Settings.gui.grid.gridHeight *
            MAST_Settings.gui.grid.yUnitSize + MAST_Const.grid.yOffsetToAvoidTearing;
        
        // If selected Prefab can be scaled
        if (MAST_Placement_Helper.GetScalable())
        {
            // Place the prefab
            GameObject placedPrefab = MAST_Placement_Place.PlacePrefabInScene();
            
            // Move prefab centerpoint between both paint areas
            placedPrefab.transform.position = (paintAreaStart + paintAreaEnd) / 2;
            
            // Scale prefab X and Z to match paint area
            Vector3 scale = new Vector3(
                Mathf.Abs(paintAreaStart.x - paintAreaEnd.x) + MAST_Settings.gui.grid.xzUnitSize,
                1,
                Mathf.Abs(paintAreaStart.z - paintAreaEnd.z) + MAST_Settings.gui.grid.xzUnitSize);
            
            placedPrefab.transform.localScale = scale;
        }
        
        // If selected Prefab cannot be scaled
        else
        {
            // Get base of rows and columns "lowest value"
            float xBase = paintAreaStart.x < paintAreaEnd.x ? paintAreaStart.x : paintAreaEnd.x;
            float zBase = paintAreaStart.z < paintAreaEnd.z ? paintAreaStart.z : paintAreaEnd.z;
            
            // Get count of rows and columns in paint area
            int xCount = (int)(Mathf.Abs(paintAreaStart.x - paintAreaEnd.x) / MAST_Settings.gui.grid.xzUnitSize);
            int zCount = (int)(Mathf.Abs(paintAreaStart.z - paintAreaEnd.z) / MAST_Settings.gui.grid.xzUnitSize);
            
            // Loop through each grid space in the area
            for (int x = 0; x <= xCount; x++)
            {
                for (int z = 0; z <= zCount; z++)
                {
                    // Set visualizer position
                    MAST_Placement_Visualizer.GetGameObject().transform.position =
                        new Vector3(xBase + (x * MAST_Settings.gui.grid.xzUnitSize),
                        MAST_Settings.gui.grid.gridHeight * MAST_Settings.gui.grid.yUnitSize,
                        zBase + (z * MAST_Settings.gui.grid.xzUnitSize));
                    
                    // Add Prefab to scene
                    MAST_Placement_Place.PlacePrefabInScene();
                }
            }
        }
        
        // Delete painting area
        DeletePaintArea();
    }
    
    // Delete paint area
    public static void DeletePaintArea()
    {
        // Set painting area to false
        paintingArea = false;
        
        // Find existing paint area and delete it - even if disabled
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.name == "MAST_Paint_Area_Visualizer")
                GameObject.DestroyImmediate(go);
        }
    }
}

#endif