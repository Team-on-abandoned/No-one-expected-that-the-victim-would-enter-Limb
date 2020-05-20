using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public class MAST_Settings_Window : EditorWindow
{
    // Have preferences changed?
    [SerializeField] private bool prefChanged = false;
    
    // GameObject reference for target parent of all placed prefeabs
    [SerializeField] private GameObject targetParent = null;
    
    private int tab = 0;
    
    [SerializeField] private Vector2 placementScrollPos = new Vector2();
    [SerializeField] private Vector2 gridScrollPos = new Vector2();
    [SerializeField] private Vector2 hotkeyScrollPos = new Vector2();
    
    private bool placementOffsetFoldout = true;
    private bool placementRotationFoldout = true;
    private bool placementRandomizerFoldout = true;
    
    private bool guiToolbarFoldout = true;
    private bool guiPaletteFoldout = true;
    private bool guiGridFoldout = true;
    
    // ---------------------------------------------------------------------------
    // Initialize
    // ---------------------------------------------------------------------------
    public void Initialize()
    {
        // Get target parent from saved target parent name
        targetParent = GameObject.Find(MAST_Settings.placement.targetParentName);
        
        // If target parent doesn't exist, create it and named it "MAST_Holder"
        if (targetParent == null)
        {
            targetParent = new GameObject();
            targetParent.transform.position = new Vector3(0, 0, 0);
            MAST_Settings.placement.targetParentName = "MAST_Holder";
            targetParent.name = MAST_Settings.placement.targetParentName;
        }
    }
    
    void OnFocus() {}
    
    void OnDestroy() {}
    
// ---------------------------------------------------------------------------
#region Preferences Interface
// ---------------------------------------------------------------------------
    void OnGUI()
    {
        // If target parent is deleted, create a new one
        if (targetParent == null)
            Initialize();
        
        // Start polling for changes
        EditorGUI.BeginChangeCheck ();
        
        string[] tabCaptions = {"Placement", "GUI", "Hotkeys"};
        
        tab = GUILayout.Toolbar (tab, tabCaptions);
        switch (tab) {
            case 0:
                PlacementGUI();
                break;
            case 1:
                GUIGUI();
                break;
            case 2:
                HotkeyGUI();
                break;
        }
        
        // If changes to UI value ocurred, update
        if (EditorGUI.EndChangeCheck ()) {
            prefChanged = true;
        }
    }
#endregion
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
#region Placement Tab GUI
// ---------------------------------------------------------------------------
    private void PlacementGUI()
    {
        // Verical scroll view for palette items
        placementScrollPos = EditorGUILayout.BeginScrollView(placementScrollPos);
        
        // ----------------------------------
        // Placement Destination
        // ----------------------------------
        targetParent = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Placement Destination",
            "Drag a GameObject from the Hierarchy into this field.  It will be used as the parent of new placed models"),
            targetParent, typeof(GameObject), true);
        MAST_Settings.placement.targetParentName = targetParent.name;
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Snap to Grid
        // ----------------------------------
        GUILayout.BeginHorizontal();
        
        MAST_Settings.placement.snapToGrid =
            GUILayout.Toggle(MAST_Settings.placement.snapToGrid, "Snap to Grid");
        
        GUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Offset
        // ----------------------------------
        placementOffsetFoldout = EditorGUILayout.Foldout(placementOffsetFoldout, "Offset Settings");
        if (placementOffsetFoldout) 
        {
            EditorGUILayout.Space();
            
            MAST_Settings.placement.offset.pos =
                EditorGUILayout.Vector3Field("Position Offset", MAST_Settings.placement.offset.pos);
            
            EditorGUILayout.Space();
            
            MAST_Settings.placement.offset.overridePrefab =
                GUILayout.Toggle(MAST_Settings.placement.offset.overridePrefab, "Override Prefab Settings");
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Rotation
        // ----------------------------------
        placementRotationFoldout = EditorGUILayout.Foldout(placementRotationFoldout, "Rotation Settings");
        if (placementRotationFoldout) 
        {
            EditorGUILayout.Space();
            
            MAST_Settings.placement.rotation.factor =
                EditorGUILayout.Vector3Field("Rotation Factor", MAST_Settings.placement.rotation.factor);
            
            EditorGUILayout.Space();
            
            MAST_Settings.placement.rotation.overridePrefab =
                GUILayout.Toggle(MAST_Settings.placement.rotation.overridePrefab, "Override Prefab Settings");
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Randomizer
        // ----------------------------------
        placementRandomizerFoldout = EditorGUILayout.Foldout(placementRandomizerFoldout, "Randomizer Settings");
        if (placementRandomizerFoldout) 
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Randomize Rotation (0 = no rotation)", EditorStyles.boldLabel);
            
            MAST_Settings.placement.randomizer.rotate =
                EditorGUILayout.Vector3Field("Rotation Factor", MAST_Settings.placement.randomizer.rotate);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Randomize Scale (1 = no scaling)", EditorStyles.boldLabel);
            
            MAST_Settings.placement.randomizer.scaleLock =
                (MAST_Placement_ScriptableObject.AxisLock)EditorGUILayout.EnumPopup(
                "Lock", MAST_Settings.placement.randomizer.scaleLock);
            
            MAST_Settings.placement.randomizer.scaleMin =
                EditorGUILayout.Vector3Field("Minimum", MAST_Settings.placement.randomizer.scaleMin);
            MAST_Settings.placement.randomizer.scaleMax =
                EditorGUILayout.Vector3Field("Maximum", MAST_Settings.placement.randomizer.scaleMax);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Randomize Position (0 = no offset)", EditorStyles.boldLabel);
            
            MAST_Settings.placement.randomizer.posMin =
                EditorGUILayout.Vector3Field("Minimum", MAST_Settings.placement.randomizer.posMin);
            MAST_Settings.placement.randomizer.posMax =
                EditorGUILayout.Vector3Field("Maximum", MAST_Settings.placement.randomizer.posMax);
            
            EditorGUILayout.Space();
            
            MAST_Settings.placement.randomizer.overridePrefab =
                GUILayout.Toggle(MAST_Settings.placement.randomizer.overridePrefab, "Override Prefab Settings");
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // Get placement settings object path from selected item in project view
        if (GUILayout.Button(
            new GUIContent("Load Placement Settings from selected file in Project",
            "Use the selected (Placement Settings) file in project")))
        {
            string path = MAST_Asset_Loader.GetPathOfSelectedObjectTypeOf(typeof(MAST_Placement_ScriptableObject));
            
            if (path != "")
            {
                MAST_Settings.core.placementSettingsPath = path;
                MAST_Settings.Load_Placement_Settings();
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
#endregion
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
#region GUI Tab GUI
// ---------------------------------------------------------------------------
    private void GUIGUI()
    {
        // Verical scroll view for palette items
        gridScrollPos = EditorGUILayout.BeginScrollView(gridScrollPos);
        
        guiToolbarFoldout = EditorGUILayout.Foldout(guiToolbarFoldout, "Toolbar Settings");
        if (guiToolbarFoldout)
        {
            MAST_Settings.gui.toolbar.position =
                (MAST_GUI_ScriptableObject.ToolbarPos)EditorGUILayout.EnumPopup(
                "Position", MAST_Settings.gui.toolbar.position);
            
            MAST_Settings.gui.toolbar.scale = EditorGUILayout.Slider(
                "Scale", MAST_Settings.gui.toolbar.scale, 0.5f, 1f);
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        guiPaletteFoldout = EditorGUILayout.Foldout(guiPaletteFoldout, "Palette Settings");
        if (guiPaletteFoldout)
        {
            MAST_Settings.gui.palette.bgColor =
                (MAST_GUI_ScriptableObject.PaleteBGColor)EditorGUILayout.EnumPopup(
                "Background Color", MAST_Settings.gui.palette.bgColor);
            
            EditorGUILayout.Space();
            
            MAST_Settings.gui.palette.snapshotCameraPitch =
                Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(
                "Thumbnail Pitch (0-360)", "Rotation around the Y axis"),
                MAST_Settings.gui.palette.snapshotCameraPitch), 0f, 360f);
            
            MAST_Settings.gui.palette.snapshotCameraYaw =
                Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(
                "Thumbnail Yaw (0-90)", "Rotation around the X axis"),
                MAST_Settings.gui.palette.snapshotCameraYaw), 0f, 90f);
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        guiGridFoldout = EditorGUILayout.Foldout(guiGridFoldout, "Grid Settings");
        if (guiGridFoldout)
        {
            EditorGUILayout.LabelField("Grid Dimensions", EditorStyles.boldLabel);
            
            MAST_Settings.gui.grid.xzUnitSize =
                EditorGUILayout.FloatField(new GUIContent(
                "X/Z Unit Size", "Size of an individual grid square for snapping"),
                MAST_Settings.gui.grid.xzUnitSize);
            MAST_Settings.gui.grid.yUnitSize =
                EditorGUILayout.FloatField(new GUIContent(
                "Y Unit Size", "Y step for grid raising/lowering"),
                MAST_Settings.gui.grid.yUnitSize);
            MAST_Settings.gui.grid.cellCount =
                EditorGUILayout.IntField(new GUIContent(
                "Count (Center to Edge)", "Count of squares from center to each edge"),
                MAST_Settings.gui.grid.cellCount);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Grid Cosmetics", EditorStyles.boldLabel);
            
            MAST_Settings.gui.grid.tintColor =
                EditorGUILayout.ColorField("Tint Color", MAST_Settings.gui.grid.tintColor);
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
        }
        
        // Get grid settings object path from selected item in project view
        if (GUILayout.Button(
            new GUIContent("Load GUI Settings from selected file in Project",
            "Use the selected (GUI Settings) file in project")))
        {
            string path = MAST_Asset_Loader.GetPathOfSelectedObjectTypeOf(typeof(MAST_GUI_ScriptableObject));
            
            if (path != "")
            {
                MAST_Settings.core.guiSettingsPath = path;
                MAST_Settings.Load_GUI_Settings();
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
#endregion
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
#region Hotkey Tab GUI
// ---------------------------------------------------------------------------
    private void HotkeyGUI()
    {
        // Verical scroll view for palette items
        hotkeyScrollPos = EditorGUILayout.BeginScrollView(hotkeyScrollPos);
        
        // ----------------------------------
        // Toggle grid On/Off
        // ----------------------------------
        EditorGUILayout.LabelField("Toggle Grid On/Off", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.toggleGridKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.toggleGridKey);
        MAST_Settings.hotkey.toggleGridMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.toggleGridMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Move grid up
        // ----------------------------------
        EditorGUILayout.LabelField("Move Grid Up", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.moveGridUpKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.moveGridUpKey);
        MAST_Settings.hotkey.moveGridUpMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.moveGridUpMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Move grid down
        // ----------------------------------
        EditorGUILayout.LabelField("Move Grid Down", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.moveGridDownKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.moveGridDownKey);
        MAST_Settings.hotkey.moveGridDownMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.moveGridDownMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Deselect prefab in palette
        // ----------------------------------
        EditorGUILayout.LabelField("Deselect Draw Tool and Palette Selection", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.deselectPrefabKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.deselectPrefabKey);
        MAST_Settings.hotkey.deselectPrefabMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.deselectPrefabMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Draw single
        // ----------------------------------
        EditorGUILayout.LabelField("Select Draw Single Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.drawSingleKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.drawSingleKey);
        MAST_Settings.hotkey.drawSingleMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.drawSingleMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Draw continuous
        // ----------------------------------
        EditorGUILayout.LabelField("Select Draw Continuous Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.drawContinuousKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.drawContinuousKey);
        MAST_Settings.hotkey.drawContinuousMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.drawContinuousMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Paint square
        // ----------------------------------
        EditorGUILayout.LabelField("Select Paint Square Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.paintSquareKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.paintSquareKey);
        MAST_Settings.hotkey.paintSquareMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.paintSquareMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Randomizer
        // ----------------------------------
        EditorGUILayout.LabelField("Select Randomizer Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.randomizerKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.randomizerKey);
        MAST_Settings.hotkey.randomizerMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.randomizerMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Erase
        // ----------------------------------
        EditorGUILayout.LabelField("Select Erase Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.eraseKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.eraseKey);
        MAST_Settings.hotkey.eraseMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.eraseMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // New random seed
        // ----------------------------------
        EditorGUILayout.LabelField("Generate New Random(izer) Seed", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.newRandomSeedKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.newRandomSeedKey);
        MAST_Settings.hotkey.newRandomSeedMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.newRandomSeedMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Rotate prefab
        // ----------------------------------
        EditorGUILayout.LabelField("Rotate Prefab", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.rotatePrefabKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.rotatePrefabKey);
        MAST_Settings.hotkey.rotatePrefabMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.rotatePrefabMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Flip prefab
        // ----------------------------------
        EditorGUILayout.LabelField("Flip Prefab", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.flipPrefabKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.flipPrefabKey);
        MAST_Settings.hotkey.flipPrefabMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.flipPrefabMod);
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // Get hotkey settings object path from selected item in project view
        if (GUILayout.Button(
            new GUIContent("Load Hotkey Settings from selected file in Project",
            "Use the selected (Hotkey Settings) file in project")))
        {
            string path = MAST_Asset_Loader.GetPathOfSelectedObjectTypeOf(typeof(MAST_Hotkey_ScriptableObject));
            
            if (path != "")
            {
                MAST_Settings.core.hotkeySettingsPath = path;
                MAST_Settings.Load_Hotkey_Settings();
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
#endregion
// ---------------------------------------------------------------------------
    
    // Return whether preferences have changed and set "preferences changed" back to false
    public bool PreferencesChanged()
    {
        if (prefChanged)
        {
            prefChanged = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}

#endif