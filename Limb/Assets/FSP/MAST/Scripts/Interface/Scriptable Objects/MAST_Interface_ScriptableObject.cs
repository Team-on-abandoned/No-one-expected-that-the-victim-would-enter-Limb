using System.Collections.Generic;
using System;
using UnityEngine;

#if (UNITY_EDITOR)

[Serializable]
public class MAST_Interface_ScriptableObject : ScriptableObject
{
    [SerializeField] public bool gridExists = false;
    [SerializeField] public int selectedDrawToolIndex = -1;
    [SerializeField] public int selectedItemIndex = -1;
    
    [SerializeField] public string lastPrefabPath = "Assets";
    
    [SerializeField] public GameObject[] prefabs;
    [SerializeField] public string[] paletteItemTooltip;
    [SerializeField] public int columnCount = 3;
}

#endif