using UnityEngine;
using System;

#if (UNITY_EDITOR)

// Use [SelectionBase] to make sure parent is selected when clicking a child the first time
[SelectionBase]
[ExecuteInEditMode]
public class MAST_Prefab_Component : MonoBehaviour
{

    
    public enum AxisLock { NONE = 0, XZ = 1, XYZ = 2 }
    
    // ---------------------------------------------------------------------------
    
    [Space(5)]
    [Header("Category Settings")]
    
    [Space(10)]
    [Tooltip("Links this prefab to other prefabs in the same category")]
    public int categoryID = 0;
    
    // ---------------------------------------------------------------------------
    
    [Space(5)]
    [Header("Placement Settings")]
    
    [Space(10)]
    [Tooltip("Allow this Prefab to be placed inside other Prefabs?")]
    public bool placeInsideOthers = true;
    
    [Space(10)]
    [Tooltip("Offset relative to position on grid")]
    public Vector3 offsetPos = new Vector3(0.0f, 0.0f, 0.0f);
    
    [Space(5)]
    [Tooltip("Degrees to rotate by.  Set to zero for no rotation")]
    public Vector3 rotationFactor = new Vector3(90f, 90f, 90f);
    
    [Space(5)]
    [Tooltip("Stretch prefabs when painting an area?")]
    public bool scalable = false;
    
    [Space(5)]
    [Tooltip("Randomizer Options")]
    public Randomizer randomizer;
    [Serializable] public class Randomizer
    {
        [Tooltip("Is prefab randomizable?")]
        public bool randomizable = false;
        
        [Tooltip("Allow this prefab to be replaced by another within the same category?")]
        public bool replaceable = false;
        
        [Tooltip("Rotation factor during randomization - 0 for no rotation")]
        public Vector3 rotate = new Vector3(1.0f, 1.0f, 1.0f);
        [Tooltip("Minimum scale during randomization")]
        public Vector3 scaleMin = new Vector3(0.75f, 0.75f, 0.75f);
        [Tooltip("Maximum scale during randomization")]
        public Vector3 scaleMax = new Vector3(1.25f, 1.25f, 1.25f);
        [Tooltip("Lock axis together during randomization?")]
        public AxisLock scaleLock = AxisLock.XZ;
        
        [Tooltip("Minimum position offset during randomization")]
        public Vector3 posMin = new Vector3(-0.5f, -0.1f, -0.5f);
        [Tooltip("Maximum position offset during randomization")]
        public Vector3 posMax = new Vector3(0.5f, 0.1f, 0.5f);
    }
    
    // ---------------------------------------------------------------------------
    
    [Space(5)]
    [Header("Other Settings")]
    
    [Space(10)]
    [Tooltip("Include prefab when merging models?")]
    public bool includeInMerge = true;
    
}
#endif