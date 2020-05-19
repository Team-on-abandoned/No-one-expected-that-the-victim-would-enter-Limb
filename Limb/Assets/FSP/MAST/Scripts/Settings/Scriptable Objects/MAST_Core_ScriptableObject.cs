using UnityEngine;

#if (UNITY_EDITOR)

public class MAST_Core_ScriptableObject : ScriptableObject
{
    [SerializeField] public string guiSettingsPath =
        MAST_Asset_Loader.GetMASTRootFolder() + "/Settings/GUI_Settings.asset";
        
    [SerializeField] public string placementSettingsPath =
        MAST_Asset_Loader.GetMASTRootFolder() + "/Settings/Placement_Settings.asset";
        
    [SerializeField] public string hotkeySettingsPath =
        MAST_Asset_Loader.GetMASTRootFolder() + "/Settings/Hotkey_Settings.asset";
}

#endif