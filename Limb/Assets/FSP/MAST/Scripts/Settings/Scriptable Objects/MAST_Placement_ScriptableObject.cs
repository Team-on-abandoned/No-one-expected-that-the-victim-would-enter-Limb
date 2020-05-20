using System;
using UnityEngine;

#if (UNITY_EDITOR)

public class MAST_Placement_ScriptableObject : ScriptableObject
{
    // Modifier enum used in conjunction with each hotkey
    public enum AxisLock { NONE = 0, XZ = 1, XYZ = 2 }
    
    [SerializeField] public string targetParentName = MAST_Const.placement.defaultTargetParentName;
    
    [SerializeField] public bool snapToGrid = true;
    
    [SerializeField] public Offset offset;
    [Serializable] public class Offset
    {
        [SerializeField] public bool overridePrefab = false;
        
        [SerializeField] public Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
    }
    
    [SerializeField] public Rotation rotation;
    [Serializable] public class Rotation
    {
        [SerializeField] public bool overridePrefab = false;
        
        [SerializeField] public Vector3 factor = new Vector3(0.0f, 90.0f, 0.0f);
    }
    
    [SerializeField] public Randomizer randomizer;
    [Serializable] public class Randomizer
    {
        [SerializeField] public bool overridePrefab = false;
        
        [SerializeField] public bool replaceable = false;
        
        [SerializeField] public Vector3 rotate = new Vector3(1.0f, 1.0f, 1.0f);
        
        [SerializeField] public Vector3 scaleMin = new Vector3(0.75f, 0.75f, 0.75f);
        [SerializeField] public Vector3 scaleMax = new Vector3(1.25f, 1.25f, 1.25f);
        [SerializeField] public AxisLock scaleLock = AxisLock.XZ;
        
        [SerializeField] public Vector3 posMin = new Vector3(-0.5f, -0.1f, -0.5f);
        [SerializeField] public Vector3 posMax = new Vector3(0.5f, 0.1f, 0.5f);
    }
}

#endif