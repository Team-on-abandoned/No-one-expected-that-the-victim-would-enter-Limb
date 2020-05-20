using UnityEngine;

#if (UNITY_EDITOR)

public static class MAST_Placement_Visualizer
{
    [SerializeField] public static GameObject visualizerGameObject = null;
    [SerializeField] public static bool visualizerOnGrid = false;
    
    // Is the mouse pointer in the sceneview
    [SerializeField] public static bool pointerInSceneview = false;
    
    public static GameObject GetGameObject()
    {
        return visualizerGameObject;
    }
    public static void SetGameObject(GameObject newVisualizer)
    {
        visualizerGameObject = newVisualizer;
    }
    
    // Create the visualizer GameObject
    public static void CreateVisualizer(GameObject selectedPrefab)
    {
        // Exit without creating, if no Prefab is selected in the palette
        if (selectedPrefab == null)
            return;
        
        // Create a new visualizer
        visualizerGameObject = GameObject.Instantiate(selectedPrefab);
        
        // Make visualizer transparent
        //visualizerGameObject.transform.MakeVisualizerTransparent();
        
        // Name it "MAST_Visualizer" incase it needs to be found later for deletion
        visualizerGameObject.name = "MAST_Visualizer";
        
        // If not selecting the Eraser
        if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
        {
            // If saved rotation is valid for the visualizer object, then apply the rotation to it
            if (IsSavedRotationValidForVisualizer())
                visualizerGameObject.transform.rotation = MAST_Placement_Manipulate.GetCurrentRotation();
        }
        
        // Set the visualizer and all it's children to be unselectable and not shown in the hierarchy
        visualizerGameObject.hideFlags = HideFlags.HideInHierarchy;
    }
    
    // Make visualizer GameObject transparent
    private static Transform MakeVisualizerTransparent(this Transform transform)
    {
        // Attempt to get reference to GameObject Renderer
        Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();
        
        // If a Renderer was found
        if (meshRenderer != null)
        {
            // Define temporary Material used to create transparent copy of GameObject Material
            Material tempMat = new Material(Shader.Find("Standard"));
            
            Material[] tempMats = new Material[meshRenderer.sharedMaterials.Length];
            
            // Loop through each material in GameObject
            for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                // Get material from GameObject
                tempMat = new Material(meshRenderer.sharedMaterials[i]);
                
                // Change Shader to "Standard"
                tempMat.shader = Shader.Find("Standard");
                
                // Make Material transparent
                tempMat.SetFloat("_Mode", 2);
                tempMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                tempMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                tempMat.SetInt("_ZWrite", 0);
                tempMat.DisableKeyword("_ALPHATEST_ON");
                tempMat.EnableKeyword("_ALPHABLEND_ON");
                tempMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                tempMat.renderQueue = 3000;
                
                Color32 tempColor = tempMat.color;
                tempColor.a = (byte)((int)tempColor.a * 0.67);
                tempMat.color = tempColor;
                
                // Replace GameObject Material with transparent one
                tempMats[i] = tempMat;
            }
            
            meshRenderer.sharedMaterials = tempMats;
        }
        
        // Recursively run this method for each child transform
        foreach(Transform child in transform)
        {
            child.MakeVisualizerTransparent();
        }
        
        return transform;
    }
    
    // See if new selected Prefab will allow the saved rotation
    private static bool IsSavedRotationValidForVisualizer()
    {
        // If there is a saved rotation
        if (MAST_Placement_Manipulate.GetCurrentRotation() != null)
        {
            // If the current saved rotation is allowed by the prefab
            //   (If allowed rotation divides evenly into current rotation)
            //       or (Both allowed and current rotations are set to 0)
            if (((int)(MAST_Placement_Manipulate.GetCurrentRotation().eulerAngles.x % MAST_Placement_Helper.GetRotationFactor().x) == 0)
              || (int)MAST_Placement_Manipulate.GetCurrentRotation().eulerAngles.x == 0 && (int)MAST_Placement_Helper.GetRotationFactor().x == 0)
                if (((int)(MAST_Placement_Manipulate.GetCurrentRotation().eulerAngles.y % MAST_Placement_Helper.GetRotationFactor().y) == 0)
                  || (int)MAST_Placement_Manipulate.GetCurrentRotation().eulerAngles.y == 0 && (int)MAST_Placement_Helper.GetRotationFactor().y == 0)
                    if (((int)(MAST_Placement_Manipulate.GetCurrentRotation().eulerAngles.z % MAST_Placement_Helper.GetRotationFactor().z) == 0)
                      || (int)MAST_Placement_Manipulate.GetCurrentRotation().eulerAngles.z == 0 && (int)MAST_Placement_Helper.GetRotationFactor().z == 0)
                    {
                        // Return true, since saved rotation is allowed
                        return true;
                    }
        }
        // Return false, since saved rotation is not allowed
        return false;
    }
    
    // Destroy current visualizer GameObject
    public static void RemoveVisualizer()
    {
        // Find existing visualizer prefab by name and delete
        // using this method to find disabled visualizer prefabs
        foreach (GameObject gameObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            if (gameObject.name == "MAST_Visualizer")
                GameObject.DestroyImmediate(gameObject);
    }
    
    // Change visualizer prefab visibility
    // Make prefab visible or invisible if pointer is in scene view on grid
    public static void SetVisualizerVisibility(bool visible)
    {
        pointerInSceneview = visible;
        
        if (visualizerGameObject != null)
            visualizerGameObject.SetActive(visible);
    }
    
    // Moves the visualizer prefab to a position based on the current mouse position
    public static void UpdateVisualizerPosition()
    {
        // If a tool is selected
        if (MAST_Placement_Interface.placementMode != MAST_Placement_Interface.PlacementMode.None)
            
            // If visualizer exists
            if (visualizerGameObject != null)
            {
                // Update visualizer position from pointer location on grid
                visualizerGameObject.transform.position =
                    MAST_Placement_Helper.GetPositionOnGridClosestToMousePointer();
                
                // If Eraser tool is not selected
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
                {
                    // Apply position offset
                    visualizerGameObject.transform.position += MAST_Placement_Helper.GetOffsetPosition();
                    
                    // If Randomizer is selected
                    if (MAST_Settings.gui.toolbar.selectedDrawToolIndex == 3)
                        // If Prefab in randomizable, apply Randomizer to transform
                        if (MAST_Placement_Helper.Randomizer.GetRandomizable())
                            visualizerGameObject = MAST_Placement_Randomizer.ApplyRandomizerToTransform(
                                visualizerGameObject, MAST_Placement_Manipulate.GetCurrentRotation());
                }
                
                // Set visualizer visibility based on if mouse over grid
                if (pointerInSceneview)
                    visualizerGameObject.SetActive(visualizerOnGrid);
            }
    }
}

#endif