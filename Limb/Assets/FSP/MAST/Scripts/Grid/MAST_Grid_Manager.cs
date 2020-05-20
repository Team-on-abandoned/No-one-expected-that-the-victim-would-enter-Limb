using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

[Serializable]
public static class MAST_Grid_Manager
{
    // ---------------------------------------------------------------------------
    #region Variable Declaration
    // ---------------------------------------------------------------------------
    
    // Grid Appearance
    [SerializeField] public static bool gridExists = false;
    
    // Grid in Scene
    [SerializeField] private static GameObject gridGameObject;
    [SerializeField] private static Material gridMaterial;
    //[SerializeField] private static GameObject gridParent; // hidden in inspector with child grid left visible so it still draws gizmolines
    
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    // Initialize
    // ---------------------------------------------------------------------------
    public static void Initialize()
    {
        
    }
    
    // ---------------------------------------------------------------------------
    #region Grid Location
    // ---------------------------------------------------------------------------
    public static void MoveGridUp()
    {
        if (gridExists)
        {
            // Move Grid Up
            MAST_Settings.gui.grid.gridHeight += 1;
            MoveGridToNewHeight();
        }
    }
    
    public static void MoveGridDown()
    {
        if (gridExists)
        {
            // Move Grid Up
            MAST_Settings.gui.grid.gridHeight -= 1;
            MoveGridToNewHeight();
        }
    }
    
    private static void MoveGridToNewHeight()
    {
        // Calculate new grid height
        float gridY = MAST_Settings.gui.grid.gridHeight * MAST_Settings.gui.grid.yUnitSize + MAST_Const.grid.yOffsetToAvoidTearing;
        gridGameObject.transform.position =
            new Vector3(gridGameObject.transform.position.x, gridY, gridGameObject.transform.position.z);
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Create/Destroy Grid
    // ---------------------------------------------------------------------------
    
    // Return if grid reference exists
    public static bool DoesGridExist()
    {
        return gridExists;
    }
    
    // Change grid visibility
    public static void ChangeGridVisibility()
    {
        if (gridExists)
            CreateGrid();
        else
            DestroyGrid();
    }
    
    // Destroy any existing grid
    public static void DestroyGrid()
    {
        // Find existing grid(s) and delete them - even if disabled
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.name == MAST_Const.grid.defaultName)
            {
                GameObject.DestroyImmediate(go);
            }
        }
        
        // Remove locked layer
        Tools.lockedLayers &= ~(1 << MAST_Const.grid.gridLayer);
        
        gridExists = false;
    }
    
    // Create grid
    public static void CreateGrid()
    {
        CreateLinkToGrid();
        
        // Lock the layer the grid is on
        Tools.lockedLayers = 1 << MAST_Const.grid.gridLayer;
        
        gridExists = true;
    }
    
    // Create link to any grid that exists, or create a new grid
    private static void CreateLinkToGrid()
    {
        gridGameObject = GameObject.Find(MAST_Const.grid.defaultName);
        
        DestroyGrid();
        CreateNewGrid();
    }
    
    // ---------------------------------------------------------------------------
    // Create a New Grid in the Hierarchy from the Grid Prefab
    // ---------------------------------------------------------------------------
    static void CreateNewGrid()
    {
        // Create new Grid GameObject
        gridGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridGameObject.transform.position = new Vector3(0f, 0f, 0f);
        gridGameObject.name = MAST_Const.grid.defaultName;
        gridGameObject.layer = 4;
        
        // Configure Grid GameObject MeshRenderer
        MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
        gridMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        gridMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        gridMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        gridMeshRenderer.receiveShadows = false;
        
        // Configure Grid GameObject Material
        if (gridMaterial == null)
        {
            gridMaterial = MAST_Asset_Loader.GetGridMaterial();
        }
        gridMaterial.SetColor("_Color", MAST_Settings.gui.grid.tintColor);
        gridMeshRenderer.material = gridMaterial;
        
        // Add MAST_Grid_Component script to grid and pass grid preferences to it
        UpdateGridSettings();
        
        // Return the grid to its last saved height
        MoveGridToNewHeight();
        
        // Hide the grid in the hierarchy
        gridGameObject.hideFlags = HideFlags.HideInHierarchy;
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Grid Settings
    // ---------------------------------------------------------------------------
    public static void UpdateGridSettings()
    {
        if (gridGameObject != null)
        {
            // Scale plane to match new grid size
            float xzUnitSize = Mathf.Max(0, Mathf.Abs(MAST_Settings.gui.grid.xzUnitSize));
            gridGameObject.transform.localScale = new Vector3(
                xzUnitSize * MAST_Settings.gui.grid.cellCount * 2f / 10f,
                1f,
                xzUnitSize * MAST_Settings.gui.grid.cellCount * 2f / 10f);
            
            // Modify grid material values
            gridMaterial.SetColor("_Color", MAST_Settings.gui.grid.tintColor);
            //gridMaterial.SetFloat("_GridUnitSize", MAST_Settings.gui.grid.xzUnitSize);
            
            // Update grid material
            MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
            gridMeshRenderer.sharedMaterial = gridMaterial;
            
            // Scale grid relative to grid size
            gridMeshRenderer.sharedMaterial.mainTextureScale =
                new Vector2(MAST_Settings.gui.grid.cellCount / 2f, MAST_Settings.gui.grid.cellCount / 2f);
            
            // Set grid offset relative to grid size "so crosshair is at 0,0"
            float gridOffset = 1 - ((float)(MAST_Settings.gui.grid.cellCount % 4) / 4f);
            gridMeshRenderer.sharedMaterial.mainTextureOffset = new Vector2(gridOffset, gridOffset);
        }
    }
    #endregion
    // ---------------------------------------------------------------------------
}

#endif