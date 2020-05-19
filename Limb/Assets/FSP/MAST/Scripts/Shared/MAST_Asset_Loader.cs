using System;
using System.IO;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public static class MAST_Asset_Loader
{
    const string imagePath = "/Images/";
    const string guiPath = "/Etc/GUISkin.guiskin";
    const string gridMaterialPath = "/Etc/Grid_Mat.mat";
    const string paintAreaMaterialPath = "/Etc/PaintArea_Mat.mat";
    const string eraserPrefabPath = "/Etc/MAST_Eraser.prefab";
    const string thumbnailCameraPath = "/Etc/Preview_Camera.prefab";
    
    // ---------------------------------------------------------------------------
    #region Initialize
    // ---------------------------------------------------------------------------
    public static void Initialize()
    {
        
    }
    
    public static string GetMASTRootFolder()
    {
        string rootPath = "";
        
        // Get string array containing all folders
        string[] allFolders = Directory.GetDirectories(
            "Assets", "*" ,
            SearchOption.AllDirectories);
        
        // Loop through each folder, find the MAST folder, and save it locally
        foreach (string folder in allFolders)
        {
            if (folder.Substring(folder.Length - 4, 4) == "MAST")
            {
                rootPath = folder;
                //Debug.Log("Found MAST at " + folder);
            }
        }
        
        return rootPath;
    }
    
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get Thumbnail Camera
    // ---------------------------------------------------------------------------
    public static GameObject GetThumbnailCamera()
    {
        return (GameObject)AssetDatabase.LoadAssetAtPath(
            GetMASTRootFolder() + thumbnailCameraPath, typeof(GameObject));
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get Image
    // ---------------------------------------------------------------------------
    public static Texture2D GetImage(string fileName)
    {
        return (Texture2D)AssetDatabase.LoadAssetAtPath(
            GetMASTRootFolder() + imagePath + fileName, typeof(Texture2D));
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get GUISkin
    // ---------------------------------------------------------------------------
    public static GUISkin GetGUISkin()
    {
        return (GUISkin)AssetDatabase.LoadAssetAtPath(
            GetMASTRootFolder() + guiPath, typeof(GUISkin));
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get Grid Stuff
    // ---------------------------------------------------------------------------
    public static Material GetGridMaterial()
    {
        return (Material)AssetDatabase.LoadAssetAtPath(GetMASTRootFolder()
            + gridMaterialPath, typeof(Material));
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get Paint Area Material
    // ---------------------------------------------------------------------------
    public static Material GetPaintAreaMaterial()
    {
        return (Material)AssetDatabase.LoadAssetAtPath(GetMASTRootFolder()
            + paintAreaMaterialPath, typeof(Material));
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get Eraser Prefab
    // ---------------------------------------------------------------------------
    public static GameObject GetEraserPrefab()
    {
        return (GameObject)AssetDatabase.LoadAssetAtPath(
            GetMASTRootFolder() + eraserPrefabPath, typeof(GameObject));
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Get GameObjects from Selected Folder
    // ---------------------------------------------------------------------------
    public static GameObject[] GetPrefabsInFolder(string prefabFolder)
    {
        // Get the GUID of every file in that folder that is of the file type prefab
        string[] GUIDOfAllPrefabsInFolder =
            AssetDatabase.FindAssets("t:prefab", new[] { prefabFolder });
        
        // Create array to store the gameObjects
        GameObject[] allPrefabsAtPath = new GameObject[GUIDOfAllPrefabsInFolder.Length];
        
        // Create the string outside the loop so it is not recreated every loop
        string pathToPrefab;
        
        for (int index = GUIDOfAllPrefabsInFolder.Length - 1; index >= 0; index--)
        {
            // Convert GUID at current index to path string
            pathToPrefab = AssetDatabase.GUIDToAssetPath(GUIDOfAllPrefabsInFolder[index]);
            
            // Get gameObject at path
            allPrefabsAtPath[index] = (GameObject)AssetDatabase.LoadAssetAtPath(pathToPrefab, typeof(GameObject));
        }
        
        return allPrefabsAtPath;
    }
    
    // ---------------------------------------------------------------------------
    // Used by GetAllGameObjectsInSelectedFolder
    // ---------------------------------------------------------------------------
    public static string GetPathOfSelectedFolder()
    {
        // This code was entirely taken from https://gist.github.com/allanolivei/9260107
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        
        return path;
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    // Get path of the selected object
    // ---------------------------------------------------------------------------
    public static string GetPathOfSelectedObjectTypeOf(Type type)
    {
        // Get selected objects as object array
        UnityEngine.Object[] objs = Selection.GetFiltered(type, SelectionMode.Assets);
        
        // If correct object type was selected, return the first selected item's path
        if (objs.Length > 0)
            return AssetDatabase.GetAssetPath(objs[0]);
        
        // If no object of this type was selected, return an empty string
        else
            return "";
    }
    
    // ---------------------------------------------------------------------------
    // Choose a folder dialog
    // ---------------------------------------------------------------------------
    public static void Show_Choose_Folder_Dialog(string dialogTitle, string defaultPath)
    {
        string path = EditorUtility.OpenFolderPanel(dialogTitle, defaultPath, "");
    }
}

#endif