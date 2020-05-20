using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)

public static class MAST_Placement_Randomizer
{
    private static Vector3 position;
    private static Vector3 rotation;
    private static Vector3 scale;
    
    // -----------------------------------------------------------------------
    // Generate new random values for placement
    // -----------------------------------------------------------------------
    public static void GenerateNewRandomSeed()
    {
        // ---------------------
        // Replace Prefab
        // ---------------------
        if (MAST_Placement_Helper.Randomizer.GetReplaceable())
            GetRandomPrefabInCategory(MAST_Placement_Helper.GetCategoryID());
        
        // ---------------------
        // Randomize Position
        // ---------------------
        
        // Create X position
        position.x =
            Random.Range(MAST_Placement_Helper.Randomizer.Position.GetMin().x,
            MAST_Placement_Helper.Randomizer.Position.GetMax().x);
        
        // Create Y position
        position.y =
            Random.Range(MAST_Placement_Helper.Randomizer.Position.GetMin().y,
            MAST_Placement_Helper.Randomizer.Position.GetMax().y);
        
        // Create Z position
        position.z =
            Random.Range(MAST_Placement_Helper.Randomizer.Position.GetMin().z,
            MAST_Placement_Helper.Randomizer.Position.GetMax().z);
        
        // ---------------------
        // Randomize Rotation
        // ---------------------
        
        // Create X rotation
        if (MAST_Placement_Helper.Randomizer.Rotation.GetAngle().x == 0f)
            rotation.x =  0f;
        else
            rotation.x = Mathf.Floor(Random.Range(0f, 360f)
                / MAST_Placement_Helper.Randomizer.Rotation.GetAngle().x)
                * MAST_Placement_Helper.Randomizer.Rotation.GetAngle().x;
        
        // Create Y rotation
        if (MAST_Placement_Helper.Randomizer.Rotation.GetAngle().y == 0f)
            rotation.y =  0f;
        else
            rotation.y = Mathf.Floor(Random.Range(0f, 360f)
                / MAST_Placement_Helper.Randomizer.Rotation.GetAngle().y)
                * MAST_Placement_Helper.Randomizer.Rotation.GetAngle().y;
        
        // Create Z rotation
        if (MAST_Placement_Helper.Randomizer.Rotation.GetAngle().z == 0f)
            rotation.z =  0f;
        else
            rotation.z = Mathf.Floor(Random.Range(0f, 360f)
                / MAST_Placement_Helper.Randomizer.Rotation.GetAngle().z)
                * MAST_Placement_Helper.Randomizer.Rotation.GetAngle().z;
        
        // ---------------------
        // Randomize Scale
        // ---------------------
        
        // Create X scale
        scale.x =
            Random.Range(MAST_Placement_Helper.Randomizer.Scale.GetMin().x,
            MAST_Placement_Helper.Randomizer.Scale.GetMax().x);
        
        // XYZ lock on, set Y scale to match X scale
        if (MAST_Placement_Helper.Randomizer.Scale.GetLock() == MAST_Placement_ScriptableObject.AxisLock.XYZ)
            scale.y = scale.x;
        
        // XYZ lock off, create Y scale
        else
            scale.y =
                Random.Range(MAST_Placement_Helper.Randomizer.Scale.GetMin().y,
                MAST_Placement_Helper.Randomizer.Scale.GetMax().y);
        
        // XYZ or XZ lock on, set Z scale to match X scale
        if (MAST_Placement_Helper.Randomizer.Scale.GetLock() != MAST_Placement_ScriptableObject.AxisLock.NONE)
            scale.z = scale.x;
        
        // XYZ and XZ locks off, create Z scale
        else
            scale.z =
                Random.Range(MAST_Placement_Helper.Randomizer.Scale.GetMin().z,
                MAST_Placement_Helper.Randomizer.Scale.GetMax().z);
    }
    
    // -----------------------------------------------------------------------
    // Get a random prefab from the same category as the selected prefab
    // -----------------------------------------------------------------------
    private static void GetRandomPrefabInCategory(int categoryID)
    {
        // Create a list to hold all viable replacement prefabs
        List<int> replacementPrefabIndexList = new List<int>();
        
        // Define MAST component script variable outside the foreach loop
        MAST_Prefab_Component mastScript;
        
        // Loop through each prefab in the palette
        for (int prefabIndex = 0; prefabIndex < MAST_Palette.GetPrefabArray().Length; prefabIndex++)
        {
            // Get the MAST component script attached to this prefab
            mastScript = MAST_Palette.GetPrefabArray()[prefabIndex].GetComponent<MAST_Prefab_Component>();
            
            // Wrap within a try/catch incase a MAST component script isn't attached to a prefab
            try
            {
                // If prefab category ID matches and prefab is replaceable, add it to replacement prefab list
                if (mastScript.categoryID == categoryID)
                    if (mastScript.randomizer.replaceable)
                        replacementPrefabIndexList.Add(prefabIndex);
            }
            catch
            {
                // Do nothing with an error.  No MAST script was attached, so no categoryID
            }
        }
        
        // Get a random number between 0 and the total amount of replacement prefabs - 1
        int replacementPrefabIndex = (int)Random.Range(0, replacementPrefabIndexList.Count);
        
        // Select the new prefab
        MAST_Palette.selectedItemIndex = replacementPrefabIndexList[replacementPrefabIndex];
        MAST_Placement_Interface.ChangeSelectedPrefab();
    }
    
    // ---------------------------------------------------------------------------
    // Apply Randomizer values to GameObject Transform
    // ---------------------------------------------------------------------------
    public static GameObject ApplyRandomizerToTransform(GameObject gameObject, Quaternion defaultRotation)
    {
        // Move ghost based on Randomizer values
        gameObject.transform.position += position;
        
        // Rotate gameobject based on Randomizer values
        gameObject.transform.rotation = defaultRotation;
        gameObject.transform.Rotate(rotation.x, rotation.y, rotation.z);
        
        // Scale ghost based on Randomizer values
        gameObject.transform.localScale = new Vector3(
            scale.x, scale.y, scale.z);
        
        return gameObject;
    }
}

#endif