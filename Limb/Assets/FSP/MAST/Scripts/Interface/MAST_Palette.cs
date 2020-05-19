using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public class MAST_Palette
{
    private static GameObject[] prefabs;
    private static Texture2D[] texture2D;
    private static string[] tooltip;
    private static GUIContent[] guiContent;
    
    public static int selectedItemIndex = -1;
    
    // ---------------------------------------------------------------------------
    // Prefab Palette
    // ---------------------------------------------------------------------------
    
    // Load all Prefabs from the selected folder and save the palette locally
    public static string LoadPrefabs(string defaultPath)
    {
        // ------------------------------------
        // Load Prefabs from Folder
        // ------------------------------------
        
        // Show choose folder dialog
        string chosenPath = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
            defaultPath, "");
        
        // Exit if no path was chosen (Cancel was clicked)
        if (chosenPath == "")
            return chosenPath;
        
        // Convert to project local path "Assets/..."
        string assetPath = chosenPath.Replace(Application.dataPath, "Assets");
        
        // Get the GUID of every file in that folder that is of the file type prefab
        string[] GUIDOfAllPrefabsInFolder =
            AssetDatabase.FindAssets("t:prefab", new[] { assetPath });
        
        // Create array to store the gameObjects
        prefabs = new GameObject[GUIDOfAllPrefabsInFolder.Length];
        
        // Create the string outside the loop so it is not recreated every loop
        string pathToPrefab;
        
        for (int index = GUIDOfAllPrefabsInFolder.Length - 1; index >= 0; index--)
        {
            // Convert GUID at current index to path string
            pathToPrefab = AssetDatabase.GUIDToAssetPath(GUIDOfAllPrefabsInFolder[index]);
            
            // Get gameObject at path
            prefabs[index] = (GameObject)AssetDatabase.LoadAssetAtPath(pathToPrefab, typeof(GameObject));
        }
        
        // ------------------------------------
        // Create Palette Items from Prefabs
        // ------------------------------------
        
        // Generate images of all prefabs and save to a Texture2D array
        texture2D = MAST_Asset_Loader.GetThumbnailCamera()
            .GetComponent<MAST_Snapshot_Camera_Component>()
            .GetThumbnails(prefabs);
        
        // Initialize guiContent and tooltip arrays
        guiContent = new GUIContent[prefabs.Length];
        tooltip = new string[prefabs.Length];
        
        // Create GUI Content from Images and Prefab names
        for (int i = 0; i < prefabs.Length; i++)
        {
            // Create tooltip from object name
            tooltip[i] = prefabs[i].name.Replace("_", "\n").Replace(" ", "\n");
            
            // Make sure texture has a transparent background
            texture2D[i].alphaIsTransparency = true;
            
            // Create GUIContent from texture and tooltip
            guiContent[i] = new GUIContent(texture2D[i], tooltip[i]);
        }
        
        return chosenPath;
    }
    
    // Set the Palette GUIContent array directly
    public static void RestorePaletteData(
        GameObject[] newPrefabs, Texture2D[] newTexture2D, string[] newTooltip)
    {
        prefabs = newPrefabs;
        texture2D = newTexture2D;
        tooltip = newTooltip;
        
        // Initialize guiContent array
        guiContent = new GUIContent[prefabs.Length];
        
        // Create GUI Content from Images and Prefab names
        for (int i = 0; i < prefabs.Length; i++)
        {
            // Make sure texture has a transparent background
            texture2D[i].alphaIsTransparency = true;
            
            // Create GUIContent from texture and tooltip
            guiContent[i] = new GUIContent(texture2D[i], tooltip[i]);
        }
    }
    
    // Does the Palette contain prefabs?
    public static bool IsReady()
    {
        return (guiContent != null && guiContent.Length > 0);
    }
    
    // Were palette images lost
    public static bool ArePaletteImagesLost()
    {
        // If array is empty, return true
        if (texture2D == null)
            return true;
        
        // If image in array is empty, return true
        return (texture2D[0] == null);
    }
    
    // Get the Palette Prefab (GameObject) array
    public static GameObject[] GetPrefabArray()
    {
        return prefabs;
    }
    
    // Get the Palette Texture2D array
    public static Texture2D[] GetTexture2DArray()
    {
        return texture2D;
    }
    
    // Get the Palette GUIContent array for display
    public static GUIContent[] GetGUIContentArray()
    {
        return guiContent;
    }
    
    // Return currently selected prefab in the palette
    public static GameObject GetSelectedPrefab()
    {
        return prefabs[selectedItemIndex];
    }
    
}

#endif