using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public static class MAST_Placement_Place
{
    // GameObject reference for target parent of all placed prefeabs
    [SerializeField] private static GameObject targetParent = null;
    
    // Placement
    [SerializeField] public static Vector3 lastPosition;
    
    public static GameObject PlacePrefabInScene()
    {
        // Make sure target parent is referenced
        ReferenceTargetParent();
        
        if (MAST_Placement_Visualizer.visualizerOnGrid && MAST_Placement_Visualizer.GetGameObject() != null &&
            MAST_Placement_Visualizer.GetGameObject() != null)
        {
            // If drawing continuous and an object was just drawn here, exit without drawing
            if (MAST_Placement_Interface.placementMode == MAST_Placement_Interface.PlacementMode.DrawContinuous)
                if (lastPosition == MAST_Placement_Visualizer.GetGameObject().transform.position)
                    return null;
            
            // If GameObject already exists here, exit without placing the block
            if (GameObjectAlreadyHere())
                return null;
            
            // Instantiate the prefab
            //GameObject newPrefab = GameObject.Instantiate(MAST_Palette.GetSelectedPrefab());
            GameObject newPrefab = (GameObject) PrefabUtility.InstantiatePrefab(MAST_Palette.GetSelectedPrefab());
            
            // Correct GameObject transform and name "to remove (clone)"
            newPrefab.transform.rotation = MAST_Placement_Visualizer.GetGameObject().transform.rotation;
            newPrefab.transform.localScale = MAST_Placement_Visualizer.GetGameObject().transform.localScale;
            newPrefab.transform.position = MAST_Placement_Visualizer.GetGameObject().transform.position;
            newPrefab.name = MAST_Palette.GetSelectedPrefab().name;
            
            // Make new prefab child of the target parent
            newPrefab.transform.parent = targetParent.transform;
            
            // Add MAST script to GameObject if it was missing it
            //if (!newPrefab.GetComponent<MAST_Prefab_Component>())
            //    newPrefab.AddComponent<MAST_Prefab_Component>();
            
            // Randomize the seed after placement
            if (MAST_Settings.gui.toolbar.selectedDrawToolIndex == 3)
                MAST_Placement_Randomizer.GenerateNewRandomSeed();
            
            // Save this GameObject position
            lastPosition = MAST_Placement_Visualizer.GetGameObject().transform.position;
            
            // Make this an Undo point, just after placing the prefab
            Undo.RegisterCreatedObjectUndo(newPrefab, "Placed new Prefab");
            
            // Return with newly created GameObject
            return newPrefab;
        }
        
        return null;
    }
    
    // Check if a GameObject already exists here and if neither can be placed inside others
    private static bool GameObjectAlreadyHere()
    {
        // Get array containing all Colliders within 1.5f square box at placement position
        Collider[] colliders =
            Physics.OverlapBox(
            MAST_Placement_Visualizer.GetGameObject().transform.position,
            new Vector3(0.75f, 0.75f, 0.75f));
        
        // Loop through each GameObject inside or colliding with this OverlapBox
        foreach (Collider collider in colliders)
        {
            // If the nearby GameObject has a parent
            if (collider.gameObject.transform.parent != null)
            {
                // Get Parent GameObject for the GameObject containing this Collider
                GameObject nearObject = collider.gameObject.transform.parent.gameObject;
                
                // If near GameObject is not the visualizer itself
                if (nearObject.name != "MAST_Visualizer")
                {
                    // If the placed GameObject shares the same Position as the near GameObject
                    if (nearObject.transform.position ==
                        MAST_Placement_Visualizer.GetGameObject().transform.position)
                    {
                        //TODO:  Add rotation check as well
                        
                        // Get the MAST Component script of the near GameObject
                        MAST_Prefab_Component nearComponent = nearObject.GetComponent<MAST_Prefab_Component>();
                        
                        // If neither the GameObject nor placed GameObject can be placed inside others 
                        if (!MAST_Placement_Helper.GetPlaceInsideOthers() && !nearComponent.placeInsideOthers)
                        {
                            // Return true "Not safe to place"
                            return true;
                        }
                    }
                }
            }
        }
        // Return false "Safe to place"
        return false;
    }
    
    // Make sure target parent is referenced
    private static void ReferenceTargetParent()
    {
        // Get target parent from saved target parent name
        targetParent = GameObject.Find(MAST_Settings.placement.targetParentName);
        
        // If target parent doesn't exist, create it and named it "MAST_Holder"
        if (targetParent == null)
        {
            targetParent = new GameObject();
            targetParent.transform.position = new Vector3(0, 0, 0);
            MAST_Settings.placement.targetParentName = MAST_Const.placement.defaultTargetParentName;
            targetParent.name = MAST_Settings.placement.targetParentName;
        }
    }
    
}

#endif