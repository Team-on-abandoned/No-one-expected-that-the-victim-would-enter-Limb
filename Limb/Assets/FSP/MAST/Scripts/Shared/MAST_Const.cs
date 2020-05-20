#if (UNITY_EDITOR)

public static class MAST_Const
{
    // Grid
    public class Grid_Class
    {
        public string defaultName = "MAST_Grid";
        public string defaultParentName = "MAST_Grid_Parent";
        public float yOffsetToAvoidTearing = -0.001f;
        public int gridLayer = 4;
    }
    public static Grid_Class grid = new Grid_Class();
    
    // Placement
    public class Placement_Class
    {
        public string defaultTargetParentName = "MAST_Holder";
    }
    public static Placement_Class placement = new Placement_Class();
    
    
}

#endif