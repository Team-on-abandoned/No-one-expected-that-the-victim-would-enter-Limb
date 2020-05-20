using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

public static class MAST_Placement_Helper
{
    // Layer that the MAST grid is set to
    [SerializeField] private static int theLayerTheGridIsOn = 1 << MAST_Const.grid.gridLayer;
    
    // MAST script component attached the GameObjects
    [SerializeField] public static MAST_Prefab_Component mastScript;
    
// ---------------------------------------------------------------------------
#region Get Mouse Position on Grid (with or without snap)
// ---------------------------------------------------------------------------
    // Converts a position on the grid object into a position snapped to the grid
    public static Vector3 GetPositionOnGridClosestToMousePointer()
    {
        // Create a ray starting from the current point the mouse is
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        
        // Raycast to grid layer
        RaycastHit hit;
        MAST_Placement_Visualizer.visualizerOnGrid =
            Physics.Raycast(ray.origin,
            ray.direction,
            out hit,
            Mathf.Infinity,
            theLayerTheGridIsOn);
        
        // Calculate closest grid position to hit
        float xPos, zPos;
        if (MAST_Settings.placement.snapToGrid)
        {
            xPos = RoundToNearestGridCenter(hit.point.x);
            zPos = RoundToNearestGridCenter(hit.point.z);
        }
        // Return true position hit
        else
        {
            xPos = hit.point.x;
            zPos = hit.point.z;
        }
        
        // Not sure if this will ever be true
        if (xPos == Mathf.Infinity || zPos == Mathf.Infinity)
        {
            xPos = 0;
            zPos = 0;
        }
        
        // Return the closest grid position
        return new Vector3(xPos, MAST_Settings.gui.grid.gridHeight * MAST_Settings.gui.grid.yUnitSize, zPos);
    }
    
    // Calculate closest position to the grid - offset to grid center
    private static float RoundToNearestGridCenter(float positionOnAxis)
    {
        return Mathf.Round(positionOnAxis / MAST_Settings.gui.grid.xzUnitSize
            + (MAST_Settings.gui.grid.xzUnitSize / 2))
            * MAST_Settings.gui.grid.xzUnitSize - (MAST_Settings.gui.grid.xzUnitSize / 2);
    }
#endregion
// ---------------------------------------------------------------------------
    
// ---------------------------------------------------------------------------
#region Get Values from Prefab or Settings
// ---------------------------------------------------------------------------
    // Get offset position
    public static int GetCategoryID()
    {
        try { return mastScript.categoryID; }
        catch { return 0; }
    }
    
    // Get offset position
    public static Vector3 GetOffsetPosition()
    {
        if (MAST_Settings.placement.offset.overridePrefab)
            return MAST_Settings.placement.offset.pos;
        else
            try { return mastScript.offsetPos; }
            catch { return MAST_Settings.placement.offset.pos; }
    }
    
    // Get rotation factors
    public static Vector3 GetRotationFactor()
    {
        if (MAST_Settings.placement.rotation.overridePrefab)
            return MAST_Settings.placement.rotation.factor;
        else
            try { return mastScript.rotationFactor; }
            catch { return MAST_Settings.placement.rotation.factor; }
    }
    
    public class Randomizer
    {
        // Is prefab randomizable?
        public static bool GetRandomizable()
        {
            if (MAST_Settings.placement.randomizer.overridePrefab)
                return true;
            else
                try { return mastScript.randomizer.randomizable; }
                catch { return true; }
        }
        
        // Is prefab replaceable?
        public static bool GetReplaceable()
        {
            try { return mastScript.randomizer.replaceable; }
            catch { return false; }
        }
        
        // Randomize rotation
        public class Rotation
        {
            public static Vector3 GetAngle()
            {
                try { return mastScript.randomizer.rotate; }
                catch { return MAST_Settings.placement.randomizer.rotate; }
            }
        }
        
        // Randomize scale
        public class Scale
        {
            public static Vector3 GetMin()
            {
                try { return mastScript.randomizer.scaleMin; }
                catch { return MAST_Settings.placement.randomizer.scaleMin; }
            }
            public static Vector3 GetMax()
            {
                try { return mastScript.randomizer.scaleMax; }
                catch { return MAST_Settings.placement.randomizer.scaleMax; }
            }
            public static MAST_Placement_ScriptableObject.AxisLock GetLock()
            {
                try { return (MAST_Placement_ScriptableObject.AxisLock)(int)mastScript.randomizer.scaleLock; }
                catch { return MAST_Settings.placement.randomizer.scaleLock; }
            }
        }
        
        // Randomize position
        public class Position
        {
            public static Vector3 GetMin()
            {
                try { return mastScript.randomizer.posMin; }
                catch { return MAST_Settings.placement.randomizer.posMin; }
            }
            public static Vector3 GetMax()
            {
                try { return mastScript.randomizer.posMax; }
                catch { return MAST_Settings.placement.randomizer.posMax; }
            }
        }
    }
    
    // Can prefab be placed inside others?
    public static bool GetPlaceInsideOthers()
    {
        try { return mastScript.placeInsideOthers; }
        catch { return true; }
    }
    
    // Can prefab be scaled?
    public static bool GetScalable()
    {
        try { return mastScript.scalable; }
        catch { return true; }
    }
#endregion
// ---------------------------------------------------------------------------

}

#endif