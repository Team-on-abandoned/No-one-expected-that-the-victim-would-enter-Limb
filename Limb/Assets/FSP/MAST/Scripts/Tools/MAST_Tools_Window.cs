using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public class MAST_Tools_Window : EditorWindow
{  
    [SerializeField] private MAST_MergeMeshes MergeMeshesClass;
    private MAST_MergeMeshes MergeMeshes
    {
        get
        {
            // Initialize MergeMeshes Class if needed and return MergeMeshesClass
            if(MergeMeshesClass == null)
                MergeMeshesClass = new MAST_MergeMeshes();
            
            return MergeMeshesClass;
        }
    }
    
    [SerializeField] private MAST_Prefab_Creator PrefabCreator;
    
    [SerializeField] private Vector2 scrollPos;
    
    void OnFocus() {}
    
    void OnDestroy() {}
    
    // ---------------------------------------------------------------------------
    #region Preferences Interface
    // ---------------------------------------------------------------------------
    void OnGUI()
    {
        // Verical scroll view for palette items
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        //if (GUILayout.Button("Old Prefab Creator"))
        //{
        //    PrepModels.CreatePrefabsFromModels();
        //}
        
        // ------------------------------------
        // Open PrefabCreator Window Button
        // ------------------------------------
        EditorGUILayout.LabelField("Generate Prefabs from your own models.  Substitute and consolidate materials used during the process.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Open Prefab Creator window", "Open Prefab Creator window")))
        {
            // If PrefabCreator window is closed, show and initialize it
            if (PrefabCreator == null)
            {
                PrefabCreator = (MAST_Prefab_Creator)EditorWindow.GetWindow(
                    typeof(MAST_Prefab_Creator),
                    false, "MAST Prefab Creator");
                
                
                PrefabCreator.minSize = new Vector2(800, 250);
            }
            
            // If PrefabCreator window is open, close it
            else
            {
                EditorWindow.GetWindow(typeof(MAST_Prefab_Creator)).Close();
            }
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Add MAST Script to Prefabs
        // ----------------------------------
        EditorGUILayout.LabelField("This will add a MAST script to each prefab.  The script is used to describe the type of object to the MAST editor.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Add MAST Script to Prefabs",
            "Create Prefabs from all models in the selected folder.")))
        {
            // Show choose folder dialog
            string chosenPath = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
                MAST_Interface_Data_Manager.state.lastPrefabPath, "");
            
            // If a folder was chosen "Cancel was not clicked"
            if (chosenPath != "")
            {
                // Save the path the user chose
                MAST_Interface_Data_Manager.state.lastPrefabPath = chosenPath;
                MAST_Interface_Data_Manager.Save_Changes_To_Disk();
                
                // Convert to project local path "Assets/..."
                string assetPath = chosenPath.Replace(Application.dataPath, "Assets");
                
                // Loop through each Prefab in folder
                foreach (GameObject prefab in MAST_Asset_Loader.GetPrefabsInFolder(assetPath))
                {
                    // Add MAST Prefab script if not already added
                    if (!prefab.GetComponent<MAST_Prefab_Component>())
                        prefab.AddComponent<MAST_Prefab_Component>();
                }
            }
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Remove MAST Components Button
        // ----------------------------------
        EditorGUILayout.LabelField("Remove all MAST Components that were attached to the children of the selected GameObject during placement.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Remove MAST Components",
            "Remove any MAST Component code attached to gameobjects during placement")))
        {
            if (EditorUtility.DisplayDialog("Are you sure?",
                "This will remove all MAST components attached to '" + Selection.activeGameObject.name + "'",
                "Remove MAST Components", "Cancel"))
            {
                // Loop through all top-level children of targetParent
                foreach (MAST_Prefab_Component prefabComponent
                    in Selection.activeGameObject.transform.GetComponentsInChildren<MAST_Prefab_Component>())
                {
                    // Remove the SMACK_Prefab_Component script
                    GameObject.DestroyImmediate(prefabComponent);
                }
            }
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Merge Meshes by Material Button
        // ----------------------------------
        EditorGUILayout.LabelField("Merge all meshes in the selected GameObject, and place them in a new GameObject.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Merge Meshes",
            "Merge all meshes by material name, resulting in one mesh for each material")))
        {
            if (EditorUtility.DisplayDialog("Are you sure?",
                "This will combine all meshes in '" + Selection.activeGameObject.name +
                "' and save them to a new GameObject.  The original GameObject will not be affected.",
                "Merge Meshes", "Cancel"))
            {
                
                GameObject targetParent = MergeMeshes.MergeMeshes(Selection.activeGameObject);
                targetParent.name = Selection.activeGameObject.name + "_Merged";
            }
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        EditorGUILayout.EndScrollView();
    }
    #endregion
    // ---------------------------------------------------------------------------
}

#endif