using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public class MAST_PrepModels
{
    // Selected folder
    private string pathSelected;
    private string pathMeshes;
    private string pathMaterials;
    private string pathPrefabs;
    
    // Original model files
    private GameObject[] sourceModels;
    
#region StripMaterials
    
    // ---------------------------------------------------------------------------
    // Desc:    Strip Materials from all models in the specified folder
    //
    // In:      String path for models
    //
    // Out:     List containing all unique materials
    // ---------------------------------------------------------------------------
    public List<Material> StripMaterials(string targetPath)
    {
        // Return empty if no path was selected
        if (targetPath == "")
            return null;
        
        // Get Models as a GameObject array
        GameObject[] models = GetModelsInFolder(targetPath);
        
        // If Models were found
        if (models != null)
        {
            List<Material> mats = new List<Material>();
            
            // Loop through each model
            for (int i = 0; i < models.Length; i++)
            {
                // Begin a recursive algorithm that iterates through all children on down
                // Returns a Material List containing only unique Materials
                mats = GetUniqueMaterials(models[i].transform, mats);
            }
            
            // Sort material list by material names
            mats = mats.OrderBy(Material=>Material.name).ToList();
            
            return mats;
        }
        
        // NO Models were found
        return null;
    }
    
    // ---------------------------------------------------------------------------
    // Desc:    Get a GameObject array from the specific folder
    //
    // Used by StripMaterials and CreatePrefabs.
    // ---------------------------------------------------------------------------
    // In:      String folder path for models
    //
    // Out:     GameObject array containing all "model" GameObjects in the folder
    // ---------------------------------------------------------------------------
    private GameObject[] GetModelsInFolder(string targetPath)
    {
        // Get all GameObject GUID's in the specified path and any subfolders of it
        string[] modelPath = GetPathOfModelsInFolder(targetPath);
        
        // If models were found
        if (modelPath != null)
        {
            // Create array to store the gameObjects
            GameObject[] modelGameObject = new GameObject[modelPath.Length];
            
            // Loop through each GameObject in the folder
            for (int i = 0; i < modelPath.Length; i++)
            {
                // Get gameObject at path
                modelGameObject[i] = (GameObject)AssetDatabase.LoadAssetAtPath(modelPath[i], typeof(GameObject));
            }
            
            return modelGameObject;
        }
        
        return null;
    }
    
    public string[] GetPathOfModelsInFolder(string targetPath)
    {
        // Get all GameObject GUID's in the specified path and any subfolders of it
        string[] GUIDOfAllGameObjectsInFolder = AssetDatabase.FindAssets("t:gameobject", new[] { targetPath });
        
        // If any models were found
        if (GUIDOfAllGameObjectsInFolder.Length > 0)
        {
            // Create string array to store the model paths
            string[] modelPath = new string[GUIDOfAllGameObjectsInFolder.Length];
            
            // Loop through each GameObject in the folder
            for (int i = GUIDOfAllGameObjectsInFolder.Length - 1; i >= 0; i--)
            {
                // Convert GUID at current index to path string
                modelPath[i] = AssetDatabase.GUIDToAssetPath(GUIDOfAllGameObjectsInFolder[i]);
            }
            
            return modelPath;
        }
        
        return null;
    }
    
    // ---------------------------------------------------------------------------
    // Desc:    Recursive search that returns a list of unique materials.
    //
    // Used by StripMaterials.
    // ---------------------------------------------------------------------------
    // In:      Transform
    //
    // Out:     Material List containing unique materials
    // ---------------------------------------------------------------------------
    private List<Material> GetUniqueMaterials(Transform transform, List<Material> mats)
    {
        // Get this GameObject's MeshRenderer
        MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
        
        // If MeshRenderer is found
        if (meshRenderer)
        {
            // Get Materials (array) in this MeshRenderer
            Material[] tempMats = meshRenderer.sharedMaterials;
            
            // Flag to show if material was found
            bool foundMat;
            
            // Loop through each Material
            for (int t = 0; t < tempMats.Length; t++)
            {
                
                // Set found material flag back to false
                foundMat = false;
                
                // Loop through each material in the Material list
                foreach (Material material in mats)
                {
                    // If material names match set found material flag to true
                    if (tempMats[t].name == material.name)
                        foundMat = true;
                }
                
                // If the Material doesn't already exist in the unique list, add it
                if (!foundMat)
                {
                    mats.Add(tempMats[t]);
                }
            }
        }
        
        // Run this method for all child transforms
        foreach (Transform childTransform in transform)
        {
            mats = GetUniqueMaterials(childTransform, mats);
        }
        
        // Return with the current Material List
        return mats;
    }
    
#endregion
    
    // ------------------------------------------------------------------------------------------------
    // Desc:    Look for a Material in a Material List
    // ------------------------------------------------------------------------------------------------
    // In:      string          targetPath      Path containing models
    //          List<Material>  sourceMat       Original Materials stripped from the models.
    //          string[]        sourceMatName   Original Material names.  Used to find the object's
    //                                            material in the primary material list.
    //          int[]           conMatPointer   Pointers linking the source material list to new materials
    //          string[]        conMatName      New Material names "shorter array with combined materials"
    //                                            used to rename the material.
    //          Material[]      conMat          Consolidated material array.  Already contains subsituted
    //                                            materials.
    //          bool            flagAddMeshCollider     Should a MeshCollider be added for each mesh
    //          bool            flagAddEmptyParent      Should an empty parent GameObject be created
    //          bool            flagSplitMeshByMat      Should meshes be separated by materials used
    //
    // Out:     Bool whether process was successful
    // ------------------------------------------------------------------------------------------------
    public bool CreatePrefabs(string targetPath, List<Material> sourceMat, List<string> sourceMatName,
                              int[] conMatPointer, string[] conMatName, Material[] conMat,
                              bool flagAddMeshCollider, bool flagAddEmptyParent, bool flagSplitMeshByMat)
    {
        // Create a new "/Meshes" folder, one it doesn't exist
        if (!AssetDatabase.IsValidFolder(targetPath + "/Meshes"))
            AssetDatabase.CreateFolder(targetPath, "Meshes");
        
        // Create a new "/Prefabs" folder, if one doesn't exist
        if (!AssetDatabase.IsValidFolder(targetPath + "/Prefabs"))
            AssetDatabase.CreateFolder(targetPath, "Prefabs");
        
        AssetDatabase.SaveAssets();
        
        // Convert source Material name array to List for easy searching
        List<string> searchMatName = new List<string>(sourceMatName);
        
        // Loop through each source Material and give each a reference to the extracted or substituted Material
        for (int i = 0; i < sourceMat.Count; i++)
        {
            sourceMat[i] = conMat[conMatPointer[i]];
        }
        
        // Get all models in the target folder
        GameObject[] model = GetModelsInFolder(targetPath);
        
        // Create new Prefab and Prefab child GameObjects
        GameObject prefabGameObject;
        GameObject prefabGameObjectChild = null;
        
        // Loop through each model
        for (int i = 0; i < model.Length; i++)
        {
            // If adding an empty parent GameObject
            if (flagAddEmptyParent)
            {
                // Create new "empty parent" GameObject
                prefabGameObject = new GameObject(model[i].name);
                
                // Create and get reference to a new GameObject from the model by using a recursive method
                prefabGameObjectChild = CreateGameObjectFromModel(model[i].transform, searchMatName, sourceMat,
                    targetPath, flagAddMeshCollider, model[i].name, 0, 0);
                
                // Make the GameObject - created from the model - a child of the new empty prefabGameObject
                prefabGameObjectChild.transform.parent = prefabGameObject.transform;
            }
            
            // If NOT adding an empty parent GameObject
            else
            {
                // Create and get reference to a new GameObject from the model by using a recursive method
                prefabGameObject = CreateGameObjectFromModel(model[i].transform, searchMatName, sourceMat,
                    targetPath, flagAddMeshCollider, model[i].name, 0, 0);
                
            }
            
            // Create a Prefab from the new GameObject, saving it as the model's name
            PrefabUtility.SaveAsPrefabAsset(prefabGameObject, targetPath + "/Prefabs/" + model[i].name + ".prefab");
            
            // Delete the Prefab from the scene/Hierarchy
            GameObject.DestroyImmediate(prefabGameObject);
        }
        
        // Display a warning.  If the user clicks "Continue"
        if (EditorUtility.DisplayDialog("Prefab Creation Complete",
            "Successfully created " + model.Length + " prefabs!  They are located in the (" + targetPath + "/Prefabs"
            + ") folder.  The extracted Materials are located in (" + targetPath + "/Materials" + ") and the Prefab Meshes are located in ("
            + targetPath + "/Meshes).  The original models are no longer required for the prefabs.",
            "Got It!"))
        {
            
        }
        
        return true;
        
    }
    
    // ------------------------------------------------------------------------------------------------
    // Desc:    Recursive search to create a GameObject from the components of a model
    // ------------------------------------------------------------------------------------------------
    // In:      Transform       modelTransform  Transform of the model being searched through
    //          List<string>    searchMatName   Original Material names.  Used to find the object's
    //          Material[]      savedMat        Direct reference to the saved mat files
    //          string          targetPath      Location of the models
    //          bool            flagAddMeshCollider     Should a Mesh Collider be added?
    //          string          saveName        Name of the model
    //          int             saveLevel       0 for parent, 1 for first child level, etc
    //          int             saveIndex       0 for first child in array, 1 for 2nd child in array
    //
    // Out:     GameObject      newGameObject   GameObject being created
    // ------------------------------------------------------------------------------------------------
    
    private GameObject CreateGameObjectFromModel(Transform modelTransform, List<string> searchMatName,
                                                 List<Material> savedMat, string targetPath, bool flagAddMeshCollider,
                                                 string saveName, int saveLevel, int saveIndex)
    {
        // Create a new GameObject to hold this model's data from this child level
        GameObject newGameObject = new GameObject();
        
        // Rename the GameObject to the model's name at this child level
        newGameObject.name = modelTransform.gameObject.name;
        
        // Get this model's MeshRenderer
        MeshRenderer modelMeshRenderer = modelTransform.gameObject.GetComponent<MeshRenderer>();
        
        // If MeshRenderer is found
        if (modelMeshRenderer)
        {
            // Create mesh path + filename (saveName + child level + child index)
            string saveMeshPath = targetPath + "/Meshes/" + saveName + "_lvl" + saveLevel + "_ndx" + saveIndex + ".asset";
            
            Mesh saveMesh = (Mesh)GameObject.Instantiate(modelTransform.gameObject.GetComponent<MeshFilter>().sharedMesh);
            
            // Save Mesh and refesh AssetsDatabase so it can be referenced again
            AssetDatabase.CreateAsset(saveMesh, saveMeshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Add MeshFilter component to the GameObject
            MeshFilter gameObjectMeshFilter = newGameObject.AddComponent<MeshFilter>();
            
            // Add the saved Mesh to this MeshFilter
            gameObjectMeshFilter.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(saveMeshPath, typeof(Mesh));
            
            // Add MeshCollider
            if (flagAddMeshCollider)
            {
                newGameObject.AddComponent<MeshCollider>();
            }
            
            // Add MeshRenderer component to the GameObject
            MeshRenderer gameObjectMeshRenderer = newGameObject.AddComponent<MeshRenderer>();
            
            // Create Material List for the GameObject
            List<Material> gameObjectMats = new List<Material>();
            
            // Loop through each material in the Material list
            foreach (Material material in modelMeshRenderer.sharedMaterials)
            {
                // Search the searchMatName List for this Material's name and add the savedMat with the same index
                gameObjectMats.Add(savedMat[searchMatName.IndexOf(material.name)]);
            }
            
            // Add material array to the GameObject's MeshRenderer
            gameObjectMeshRenderer.sharedMaterials = gameObjectMats.ToArray();
        }
        
        // Apply transforms to the GameObject
        newGameObject.transform.position = modelTransform.position;
        newGameObject.transform.rotation = modelTransform.rotation;
        newGameObject.transform.localScale = modelTransform.lossyScale;
        
        // Create new child GameObject variable for use in recursive search
        GameObject newChildGameObject;
        
        // Run this method for all child transforms
        for (int i = 0; i < modelTransform.childCount; i++)
        {
            // Create a new GameObject for this child GameObject in the model
            newChildGameObject = CreateGameObjectFromModel(modelTransform.GetChild(i), searchMatName, savedMat,
                targetPath, flagAddMeshCollider, saveName, saveLevel + 1, i);
            
            // Attach it as a child of this new GameObject
            newChildGameObject.transform.parent = newGameObject.transform;
        }
        
        // Return this new GameObject
        return newGameObject;
    }
    
}

#endif