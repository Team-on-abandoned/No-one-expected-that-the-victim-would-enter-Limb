using UnityEngine;

#if (UNITY_EDITOR)

[System.Serializable]
public class MAST_Hotkey_ScriptableObject : ScriptableObject
{
    // Modifier enum used in conjunction with each hotkey
    public enum Modifier { NONE = 0, SHIFT = 1 }
    
    [SerializeField] public KeyCode drawSingleKey = KeyCode.D;
    [SerializeField] public Modifier drawSingleMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode drawContinuousKey = KeyCode.C;
    [SerializeField] public Modifier drawContinuousMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode paintSquareKey = KeyCode.P;
    [SerializeField] public Modifier paintSquareMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode randomizerKey = KeyCode.X;
    [SerializeField] public Modifier randomizerMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode eraseKey = KeyCode.E;
    [SerializeField] public Modifier eraseMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode newRandomSeedKey = KeyCode.X;
    [SerializeField] public Modifier newRandomSeedMod = Modifier.NONE;
    
    [SerializeField] public KeyCode toggleGridKey = KeyCode.G;
    [SerializeField] public Modifier toggleGridMod = Modifier.NONE;
    
    [SerializeField] public KeyCode moveGridUpKey = KeyCode.W;
    [SerializeField] public Modifier moveGridUpMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode moveGridDownKey = KeyCode.S;
    [SerializeField] public Modifier moveGridDownMod = Modifier.SHIFT;
    
    [SerializeField] public KeyCode deselectPrefabKey = KeyCode.Escape;
    [SerializeField] public Modifier deselectPrefabMod = Modifier.NONE;
    
    [SerializeField] public KeyCode rotatePrefabKey = KeyCode.Space;
    [SerializeField] public Modifier rotatePrefabMod = Modifier.NONE;
    
    [SerializeField] public KeyCode flipPrefabKey = KeyCode.F;
    [SerializeField] public Modifier flipPrefabMod = Modifier.NONE;
}

#endif