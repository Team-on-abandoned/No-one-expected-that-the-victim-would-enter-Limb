using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)

public class MAST_MergeMeshes
{
    
    class MeshCombine
    {
        public List<CombineInstance> combineList;
    }
    
    public GameObject MergeMeshes(GameObject source)
    {
        // Get a copy of the source GameObject
        GameObject sourceParent = GameObject.Instantiate(source);
        
        // ---------------------------------------------------------------------------
        // Remove GameObjects to Exclude from the Merge and move them back later
        // ---------------------------------------------------------------------------
        
        // Create GameObject that will hold all models to exclude from the Merge
        GameObject excludeFromMergeParent = new GameObject("Not Merged");
        
        // Get all child Transforms in the Source GameObject
        Transform[] sourceTransforms = sourceParent.GetComponentsInChildren<Transform>();
        
        // Loop through each child Transform in the Source GameObject
        for (int i = sourceTransforms.Length - 1; i >= 0; i--)
        {
            // If not including this GameObject in the MeshCombine
            if (!IncludeInMerge(sourceTransforms[i].gameObject))
            {
                sourceTransforms[i].parent = excludeFromMergeParent.transform;
            }
        }
        
        // Get all MeshFilters and MeshRenderers from the source GameObject's children
        MeshFilter[] sourceMeshFilters = sourceParent.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] sourceMeshRenderers = sourceParent.GetComponentsInChildren<MeshRenderer>();
        
        // ---------------------------------------------------------------------------
        // Get Unique Material List
        // ---------------------------------------------------------------------------
        
        // Create a List containing all unique Materials in the GameObjects
        List<Material> uniqueMats = new List<Material>();
        bool foundMat;
        
        // Loop through each MeshRenderer
        for (int i = 0; i < sourceMeshRenderers.Length; i++)
        {
            // Loop through each MeshRenderer's SharedMaterials
            for (int j = 0; j < sourceMeshRenderers[i].sharedMaterials.Length; j++)
            {
                // Set Found Material flag to "False"
                foundMat = false;
                
                // Loop through all Materials in the Unique Material list
                for (int k = 0; k < uniqueMats.Count; k++)
                {
                    // If Material was found, set the Found Material flag to "True"
                    if (sourceMeshRenderers[i].sharedMaterials[j].name == uniqueMats[k].name)
                        foundMat = true;
                }
                
                // If Found Material flag is "True", add the Material to the Unique Material Array
                if (!foundMat)
                {
                    uniqueMats.Add (sourceMeshRenderers[i].sharedMaterials[j]);
                }
            }
        }
        
        // ---------------------------------------------------------------------------
        // Extract Meshes into Separate CombineInstances based on Material
        // ---------------------------------------------------------------------------
        
        // Create a MeshCombine Class Array the size of the uniqueMats List and initialize
        MeshCombine[] uniqueMatMeshCombine = new MeshCombine[uniqueMats.Count];
        for (int i = 0; i < uniqueMats.Count; i++)
        {
            uniqueMatMeshCombine[i] = new MeshCombine();
            uniqueMatMeshCombine[i].combineList = new List<CombineInstance>();
        }
        
        // Prepare variables
        CombineInstance combineInstance;
        
        // Loop through each MeshRenderer in sourceMeshRenderers
        for (int i = 0; i < sourceMeshRenderers.Length; i++)
        {
            // Loop through each Material in each MeshRenderer
            for (int j = 0; j < sourceMeshRenderers[i].sharedMaterials.Length; j++)
            {
                // Loop through each Material in the uniqueMats List
                for (int k = 0; k < uniqueMats.Count; k++)
                {
                    // If this Material matches the Material in the uniqueMats List
                    if (sourceMeshRenderers[i].sharedMaterials[j] == uniqueMats[k])
                    {
                        // Initialize a Combine Instance
                        combineInstance = new CombineInstance();
                        
                        // Copy this mesh to the Combine Instance
                        combineInstance.mesh = sourceMeshFilters[i].sharedMesh;
                        
                        // Set it to only include the Mesh with the specified material
                        combineInstance.subMeshIndex = j;
                        
                        // Transform to world matrix
                        combineInstance.transform = sourceMeshFilters[i].transform.localToWorldMatrix;
                        
                        // Add this CombineInstance to the appropriate CombineInstance List (by Material)
                        uniqueMatMeshCombine[k].combineList.Add(combineInstance);
                    }
                }
            }
        }
        
        // ---------------------------------------------------------------------------
        // Combine all Mesh Instances into a single GameObject
        // ---------------------------------------------------------------------------
        
        // Disable all Source GameObjects
        for (int i = 0; i < sourceMeshFilters.Length; i++)
        {
            sourceMeshFilters[i].gameObject.SetActive(false);
        }
        
        // Create the final GameObject that will hold all the other GameObjects
        GameObject finalGameObject = new GameObject("Merged Meshes");
        
        // Create a new GameObject Array the size of the All Materials List
        GameObject[] singleMatGameObject = new GameObject[uniqueMats.Count];
        
        // Combine meshes for each singleMatGameObject into one mesh
        CombineInstance[] finalCombineInstance = new CombineInstance[uniqueMats.Count];
        
        // Enable all Source GameObjects
        for (int i = 0; i < sourceMeshFilters.Length; i++)
        {
            sourceMeshFilters[i].gameObject.SetActive(true);
        }
        
        // Prepare mesh filter and mesh renderer arrays for the final combine
        MeshFilter[] meshFilter = new MeshFilter[uniqueMats.Count];
        MeshRenderer[] meshRenderer = new MeshRenderer[uniqueMats.Count];
        
        // Loop through each Material in the uniqueMats List
        for (int i = 0; i < uniqueMats.Count; i++)
        {
            // Initialize GameObject
            singleMatGameObject[i] = new GameObject();
            
            singleMatGameObject[i].name = uniqueMats[i].name;
            
            // Add a MeshRender and set the Material
            meshRenderer[i] = singleMatGameObject[i].AddComponent<MeshRenderer>();
            meshRenderer[i].sharedMaterial = uniqueMats[i];
            
            // Add a MeshFilter and add the Combined Meshes with this Material
            meshFilter[i] = singleMatGameObject[i].AddComponent<MeshFilter>();
            meshFilter[i].sharedMesh = new Mesh();
            meshFilter[i].sharedMesh.CombineMeshes(uniqueMatMeshCombine[i].combineList.ToArray());
            
            // Add this Mesh to the final Mesh Combine
            finalCombineInstance[i].mesh = meshFilter[i].sharedMesh;
            finalCombineInstance[i].transform = meshFilter[i].transform.localToWorldMatrix;
            
            // Hide the GameObject
            meshFilter[i].gameObject.SetActive(false);
            
            GameObject.DestroyImmediate(singleMatGameObject[i]);
        }
        
        // Add MeshFilter to final GameObject and Combine all Meshes
        MeshFilter finalMeshFilter = finalGameObject.AddComponent<MeshFilter>();
        finalMeshFilter.sharedMesh = new Mesh();
        finalMeshFilter.sharedMesh.CombineMeshes(finalCombineInstance, false, false);
        
        // Add MeshRenderer to final GameObject Attach Materials
        MeshRenderer finalMeshRenderer = finalGameObject.AddComponent<MeshRenderer>();
        finalMeshRenderer.sharedMaterials = uniqueMats.ToArray();
        
        // Name finalGameObject and make it the child of an empty parent
        GameObject finalGameObjectParent = new GameObject(sourceParent.name + " Merged");
        finalGameObject.transform.parent = finalGameObjectParent.transform;
        
        // If the Unmerged GameObject holder is isn't empty, make it a child of the empty parent
        if (excludeFromMergeParent.transform.childCount > 0)
            excludeFromMergeParent.transform.parent = finalGameObjectParent.transform;
        
        // If the Unmerged GameObject holder is empty, delete it
        else
            GameObject.DestroyImmediate(excludeFromMergeParent);
        
        // Delete unneeded GameObjects
        GameObject.DestroyImmediate(sourceParent);
        
        // Return the complete GameObject
        return finalGameObjectParent;
    }
    
    
    private bool IncludeInMerge(GameObject go)
    {
        // If prefab is not supposed to be included in the merge, don't include its material name
        MAST_Prefab_Component prefabComponent = go.GetComponent<MAST_Prefab_Component>();
        if (prefabComponent != null)
            return prefabComponent.includeInMerge;
        
        // If no MAST prefab component was attached, include it in the merge
        return true;
    }
    
}

#endif