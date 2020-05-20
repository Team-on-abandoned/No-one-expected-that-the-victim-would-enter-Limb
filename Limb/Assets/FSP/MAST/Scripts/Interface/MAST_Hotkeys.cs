using UnityEngine;

#if (UNITY_EDITOR)

public class MAST_Hotkeys
{
    public bool ProcessHotkeys()
    {
        // Set change made to false
        bool changeMade = false;
        
        // Get current event
        Event currentEvent = Event.current;
        
        // Get the control's ID
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        
        // If a key is pressed
        if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
        {
            // Toggle grid visibility
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.toggleGridKey,
                MAST_Settings.hotkey.toggleGridMod))
            {
                MAST_Grid_Manager.gridExists = !MAST_Grid_Manager.gridExists;
                MAST_Grid_Manager.ChangeGridVisibility();
                changeMade = true;
            }
            // Move grid up
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.moveGridUpKey,
                MAST_Settings.hotkey.moveGridUpMod))
            {
                MAST_Grid_Manager.MoveGridUp();
                changeMade = true;
            }
            // Move grid down
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.moveGridDownKey,
                MAST_Settings.hotkey.moveGridDownMod))
            {
                MAST_Grid_Manager.MoveGridDown();
                changeMade = true;
            }
            // Deselect prefab in palette or draw tool
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.deselectPrefabKey,
                MAST_Settings.hotkey.deselectPrefabMod))
            {
                // Deselect palette item and draw tool
                MAST_Palette.selectedItemIndex = -1;
                MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                changeMade = true;
            }
            // Draw single
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.drawSingleKey,
                MAST_Settings.hotkey.drawSingleMod))
            {
                // If Draw Single isn't selected, select it
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 0)
                {
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex = 0;
                    MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.DrawSingle);
                }
                else
                {
                    // If Draw Single was selected, deselect it
                    if(MAST_Settings.gui.toolbar.selectedDrawToolIndex == 0)
                    {
                        MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                    }
                }
                changeMade = true;
            }
            // Draw Continuous
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.drawContinuousKey,
                MAST_Settings.hotkey.drawContinuousMod))
            {
                // If Draw Continuous isn't selected, select it
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 1)
                {
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex = 1;
                    MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.DrawContinuous);
                }
                else
                {
                    // If Draw continuous was selected, deselect it
                    if(MAST_Settings.gui.toolbar.selectedDrawToolIndex == 1)
                    {
                        MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                    }
                }
                changeMade = true;
            }
            // Paint square
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.paintSquareKey,
                MAST_Settings.hotkey.paintSquareMod))
            {
                // If Paint Square isn't selected, select it
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 2)
                {
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex = 2;
                    MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.PaintArea);
                }
                else
                {
                    // If Paint Square was selected, deselect it
                    if(MAST_Settings.gui.toolbar.selectedDrawToolIndex == 2)
                    {
                        MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                    }
                }
                changeMade = true;
            }
            // Toggle randomizer
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.randomizerKey,
                MAST_Settings.hotkey.randomizerMod))
            {
                // If Randomizer isn't selected, select it
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 3)
                {
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex = 3;
                    MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.Randomize);
                }
                else
                {
                    // If Randomizer was selected, deselect it
                    if(MAST_Settings.gui.toolbar.selectedDrawToolIndex == 3)
                    {
                        MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                    }
                }
                changeMade = true;
            }
            // Toggle erase
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.eraseKey,
                MAST_Settings.hotkey.eraseMod))
            {
                // If Erase isn't selected, select it
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
                {
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex = 4;
                    MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.Erase);
                }
                else
                {
                    // If Erase was selected, deselect it
                    if(MAST_Settings.gui.toolbar.selectedDrawToolIndex == 4)
                    {
                        MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                    }
                }
                changeMade = true;
            }
            // New random seed
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.newRandomSeedKey,
                MAST_Settings.hotkey.newRandomSeedMod))
            {
                MAST_Placement_Randomizer.GenerateNewRandomSeed();
                changeMade = true;
            }
            // Rotate prefab
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.rotatePrefabKey,
                MAST_Settings.hotkey.rotatePrefabMod))
            {
                MAST_Placement_Manipulate.RotateObject();
                changeMade = true;
            }
            // Flip prefab
            if (KeysPressed(currentEvent,
                MAST_Settings.hotkey.flipPrefabKey,
                MAST_Settings.hotkey.flipPrefabMod))
            {
                MAST_Placement_Manipulate.FlipObject();
                changeMade = true;
            }
            
        }
        
        return changeMade;
    }
    
    // All these key methods could be grouped up a lot nicer later using delegates and refs
    private bool KeysPressed(Event currentEvent, KeyCode key, MAST_Hotkey_ScriptableObject.Modifier mod)
    {
        // If correct key was pressed for the first time
        if (currentEvent.keyCode == key)
        {
            // If the correct modifier was held down
            if (IsModifierPressed(currentEvent, mod))
                return true;
        }
        
        return false;
    }
    
    // Return true if the correct modifier key is held down
    private bool IsModifierPressed(Event currentEvent, MAST_Hotkey_ScriptableObject.Modifier modifier)
    {
        // If SHIFT is needed and is held down, return true
        if (modifier == MAST_Hotkey_ScriptableObject.Modifier.SHIFT && currentEvent.shift)
            return true;
        
        // If SHIFT is not needed and SHIFT is not held down, return true
        else if (modifier == MAST_Hotkey_ScriptableObject.Modifier.NONE && !currentEvent.shift)
            return true;
        
        return false;
    }
}

#endif