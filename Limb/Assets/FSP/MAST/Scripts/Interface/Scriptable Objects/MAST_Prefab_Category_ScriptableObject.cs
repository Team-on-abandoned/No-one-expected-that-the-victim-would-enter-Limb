using System;
using UnityEngine;

#if (UNITY_EDITOR)

// THIS WILL BE USED LATER IN MAST DEVELOPMENT
public class MAST_Prefab_Category_ScriptableObject : ScriptableObject
{
    [SerializeField] public Category[] category;
    [Serializable] public class Category
    {
        [SerializeField] public int id;
        [SerializeField] public string name;
    }
    
    [SerializeField] public string[] paletteItemTooltip;
}

#endif