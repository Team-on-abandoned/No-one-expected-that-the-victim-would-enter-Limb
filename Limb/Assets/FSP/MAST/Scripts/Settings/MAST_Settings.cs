using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

[Serializable]
public static class MAST_Settings
{ 
    [SerializeField] public static MAST_Core_ScriptableObject core;
    [SerializeField] public static MAST_GUI_ScriptableObject gui;
    [SerializeField] public static MAST_Placement_ScriptableObject placement;
    [SerializeField] public static MAST_Hotkey_ScriptableObject hotkey;
    
    // ---------------------------------------------------------------------------
    // When class is enabled
    // ---------------------------------------------------------------------------
    public static void Initialize()
    {
        // Load settings from scriptable object if it was lost
        if (core == null)
            Load_Settings();
    }
    
    static void OnDisable() {}
    
    static void OnFocus() {}
    
    static void OnDestroy() {}
    
    // ---------------------------------------------------------------------------
    #region Manage Settings Scriptable Object
    // ---------------------------------------------------------------------------
    public static void Load_Settings()
    {
        Load_Core_Settings();
        Load_GUI_Settings();
        Load_Placement_Settings();
        Load_Hotkey_Settings();
        
        // Save the scriptable object
        AssetDatabase.SaveAssets();
    }
    
    public static void Load_Core_Settings()
    {
        // Get MAST Core path
        string corePath = MAST_Asset_Loader.GetMASTRootFolder() + "/Etc/MAST_Core_Settings.asset";
        
        // Load the MAST Core scriptable object
        core = AssetDatabase.LoadAssetAtPath<MAST_Core_ScriptableObject>(corePath);
        
        // If the Core scriptable object isn't found, create a new default one
        if (core == null)
        {
            core = ScriptableObject.CreateInstance<MAST_Core_ScriptableObject>();
            AssetDatabase.CreateAsset(core, corePath);
        }
    }
    
    public static void Load_GUI_Settings()
    {
        // Get/create GUI scriptable object
        gui = AssetDatabase.LoadAssetAtPath<MAST_GUI_ScriptableObject>(core.guiSettingsPath);
        
        // If scripable object doesn't exist, create a default scripable object
        if (gui == null)
        {
            gui = ScriptableObject.CreateInstance<MAST_GUI_ScriptableObject>();
            AssetDatabase.CreateAsset(gui, core.guiSettingsPath);
        }
    }
    
    public static void Load_Placement_Settings()
    {
        // Get/create Placement scriptable object
        placement = AssetDatabase.LoadAssetAtPath<MAST_Placement_ScriptableObject>(core.placementSettingsPath);
        
        // If scripable object doesn't exist, create a default scripable object
        if (placement == null)
        {
            placement = ScriptableObject.CreateInstance<MAST_Placement_ScriptableObject>();
            AssetDatabase.CreateAsset(placement, core.placementSettingsPath);
        }
    }
    
    public static void Load_Hotkey_Settings()
    {
        // Get/create Hotkey scriptable object
        hotkey = AssetDatabase.LoadAssetAtPath<MAST_Hotkey_ScriptableObject>(core.hotkeySettingsPath);
        
        // If scripable object doesn't exist, create a default scripable object
        if (hotkey == null)
        {
            hotkey = ScriptableObject.CreateInstance<MAST_Hotkey_ScriptableObject>();
            AssetDatabase.CreateAsset(hotkey, core.hotkeySettingsPath);
        }
    }
    
    // Save preferences to a scriptable object
    public static void Save_Settings()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.SetDirty(core);
        EditorUtility.SetDirty(gui);
        EditorUtility.SetDirty(placement);
        EditorUtility.SetDirty(hotkey);
    }
    
    #endregion
    // ---------------------------------------------------------------------------
}

#endif