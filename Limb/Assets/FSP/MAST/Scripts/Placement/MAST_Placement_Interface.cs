using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

public static class MAST_Placement_Interface
{
    // Draw Tools
    public enum PlacementMode
    {
        None, DrawSingle, DrawContinuous, PaintArea, Randomize, Erase
    }
    [SerializeField] public static PlacementMode placementMode = PlacementMode.None;
    
// ---------------------------------------------------------------------------
#region Change Placement Mode
// ---------------------------------------------------------------------------
    public static void ChangePlacementMode(PlacementMode newPlacementMode)
    {
        // Get new selected Draw Tool
        placementMode = newPlacementMode;
        
        // Remove any previous visualizer
        MAST_Placement_Visualizer.RemoveVisualizer();
        
        // --------------------------------
        // Create Visualizer
        // --------------------------------
        
        // If changed tool to Nothing or Eraser
        if (placementMode == PlacementMode.None || placementMode == PlacementMode.Erase)
        {
            // Deselect any item in the palette
            MAST_Palette.selectedItemIndex = -1;
            
            // If changed tool to Eraser
            if (placementMode == PlacementMode.Erase)
            {
                // Create eraser visualizer
                ChangePrefabToEraser();
            }
            
            // If changed tool to Nothing, remove the visualizer
            else
            {
                MAST_Placement_Visualizer.RemoveVisualizer();
            }
        }
        
        // If changed tool to Draw Single, Draw Continuous, Paint Area, or Randomizer
        else
        {
            // If a palette item is selected
            if (MAST_Palette.selectedItemIndex != -1)
            {
                // Create visualizer from selected item in the palette
                ChangeSelectedPrefab();
                
                // If changed tool to Randomizer
                if (placementMode == PlacementMode.Randomize)
                {
                    // Make a new random seed
                    MAST_Placement_Randomizer.GenerateNewRandomSeed();
                }
            }
        }
        
        // If Draw Continuous nor Paint Area tools are selected
        if (placementMode != PlacementMode.DrawContinuous &&
            placementMode != PlacementMode.PaintArea)
        {
            // Delete last saved position
            MAST_Placement_Place.lastPosition = Vector3.positiveInfinity;
            
            // Remove any paint area visualization
            MAST_Placement_PaintArea.DeletePaintArea();
        }
        
    }
#endregion
    
    // Change visualizer prefab when a new item is selected in the palette menu
    public static void ChangeSelectedPrefab()
    {
        // Get reference to the MAST script attached to the GameObject
        MAST_Placement_Helper.mastScript = 
            MAST_Palette.GetSelectedPrefab().GetComponent<MAST_Prefab_Component>();
        
        // Remove any existing visualizer
        MAST_Placement_Visualizer.RemoveVisualizer();
        
        // Create a new visualizer
        MAST_Placement_Visualizer.CreateVisualizer(MAST_Palette.GetSelectedPrefab());
    }
    
    // Change visualizer to eraser
    public static void ChangePrefabToEraser()
    {
        // Remove any existing visualizer
        MAST_Placement_Visualizer.RemoveVisualizer();
        
        // Create a new visualizer with eraser
        MAST_Placement_Visualizer.CreateVisualizer(MAST_Asset_Loader.GetEraserPrefab());
    }
    
    public static void ErasePrefab()
    {
        // Get array containing all Colliders within eraser
        Collider[] colliders =
            Physics.OverlapBox(
                MAST_Placement_Visualizer.GetGameObject().transform.position +
                new Vector3(0f, 0.35f, 0f), new Vector3(0.4f, 0.4f, 0.4f));
        
        // Loop through each GameObject inside or colliding with this OverlapBox
        foreach (Collider collider in colliders)
        {
            // Use try/catch, incase this collider's GameObject is already destroyed
            try
            {
                // If the nearby GameObject has a parent
                if (collider.gameObject.transform.parent != null)
                {
                    // Get Parent GameObject for the GameObject containing this Collider
                    GameObject objectToDelete = GetPrefabParent(collider.gameObject.transform).gameObject;
                    
                    // If a GameObject placed with MAST was found
                    if (objectToDelete != null)
                    {
                        // If near GameObject is not the visualizer itself
                        if (objectToDelete.name != "MAST_Visualizer" &&
                            objectToDelete.name != MAST_Const.grid.defaultName &&
                            objectToDelete.name != MAST_Const.grid.defaultParentName)
                        {
                            // Erase it, but allow an undo
                            Undo.DestroyObjectImmediate(objectToDelete);
                        }
                    }
                }
            }
            catch
            {
                // Do nothing since this collider's GameObject was already destroyed
            }
        }
    }
    
    // Get Prefab parent of provided transform
    private static Transform GetPrefabParent(Transform transform)
    {
        // If this GameObject doesn't have a MAST_Prefab_Component script
        if (transform.gameObject.GetComponent<MAST_Prefab_Component>() == null)
        {
            // Get result from GameObject parent or if at the top-level, return null
            try { return GetPrefabParent(transform.parent); }
            catch { return null; }
        }
        
        // If this GameObject has a MAST_Prefab_Component script, return it's transform
        else
            return transform;
    }
}

#endif