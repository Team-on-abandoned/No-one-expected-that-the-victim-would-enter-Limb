using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

#if (UNITY_EDITOR)

// This class largely contains code from:
// http://answers.unity3d.com/questions/1213025/separating-submeshes-into-unique-meshes.html

// For MAST, rewrote the main loop, changing it to Singleton and returning a single GameObject containing
// child Meshes, instead of saving the Meshes separately.  Other various fixes/improvements.

public class MAST_SubMeshSplitter
{
    public class MeshFromSubmesh
    {
        public Mesh mesh;
        public int id; // Represent the ID of the sub mesh from with the new 'mesh' has been created
    }
    
    // ------------------------------------------------------------------------
    // Split SubMeshes into separate Meshes and return a full GameObject
    // containing separate Meshes with the correct Materials assigned
    // ------------------------------------------------------------------------
    public static GameObject GetGameObjectWithSplitSubmeshes(GameObject sourceGameObject)
    {
        // Get an array of MeshFilters and MeshRenderers
        MeshFilter[] meshFilters = sourceGameObject.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] meshRenderers = sourceGameObject.GetComponentsInChildren<MeshRenderer>();
        
        // Create a reference for the parent GameObject containing all child GameObjects
        GameObject targetParent = new GameObject();
        
        // Create reference for child GameObject that will hold the MeshFilter and MeshRenderer
        // for each SubMesh split from the main Mesh.
        GameObject targetChild;
        
        // Index of SubMesh for naming new Meshes, counts up as each new SubMesh is added 
        int subMeshIndex = 0;
        
        // Loop through each MeshFilter
        for ( int i = 0; i < meshFilters.Length; i++ )
        {
            // Split the Mesh into a SubMesh list
            List<MeshFromSubmesh> meshFromSubmeshes =
                GetAllSubMeshAsIsolatedMeshes(meshFilters[i].sharedMesh);
            
            // Loop through each SubMesh
            for ( int t = 0; t < meshFromSubmeshes.Count; t++ )
            {
                // Create new Mesh GameObject
                targetChild = new GameObject();
                
                // Make this GameObject a child of TargetParent
                targetChild.transform.SetParent(targetParent.transform);
                
                // Add MeshFilter and add SubMesh to the child GameObject
                MeshFilter meshFilter = targetChild.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = meshFromSubmeshes[t].mesh;
                
                // Name the new mesh
                subMeshIndex++;
                meshFilter.sharedMesh.name = sourceGameObject.name + "_Submesh_" + subMeshIndex;
                
                // Add MeshRenderer and add Material to it
                MeshRenderer meshRenderer = targetChild.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = meshRenderers[i].sharedMaterials[meshFromSubmeshes[t].id];
                
                // Change child GameObject name to match the Material
                targetChild.name = meshRenderer.sharedMaterial.name;
                
            }
        }
        
        return targetParent;
    }
    
    // ------------------------------------------------------------------------

    private static List<MeshFromSubmesh> GetAllSubMeshAsIsolatedMeshes( Mesh mesh )
    {
        List<MeshFromSubmesh> meshesToReturn = new List<MeshFromSubmesh>();
        if ( !mesh )
        {
            Debug.LogError( "No mesh passed into GetAllSubMeshAsIsolatedMeshes!" );
            return meshesToReturn;
        }
        
        int submeshCount = mesh.subMeshCount;
        
        MeshFromSubmesh m1;
        
        for ( int i = 0; i < submeshCount; i++ )
        {
            m1 = new MeshFromSubmesh();
            m1.id = i;
            m1.mesh = mesh.GetSubmesh( i );
            meshesToReturn.Add( m1 );
        }
        return meshesToReturn;
    }
}

public static class MeshExtension
{
    private class Vertices
    {
        List<Vector3> verts = null;
        List<Vector2> uv1 = null;
        List<Vector2> uv2 = null;
        List<Vector2> uv3 = null;
        List<Vector2> uv4 = null;
        List<Vector3> normals = null;
        List<Vector4> tangents = null;
        List<Color32> colors = null;
        List<BoneWeight> boneWeights = null;
        
        public Vertices()
        {
            verts = new List<Vector3>();
        }
        public Vertices( Mesh aMesh )
        {
            verts = CreateList( aMesh.vertices );
            uv1 = CreateList( aMesh.uv );
            uv2 = CreateList( aMesh.uv2 );
            uv3 = CreateList( aMesh.uv3 );
            uv4 = CreateList( aMesh.uv4 );
            normals = CreateList( aMesh.normals );
            tangents = CreateList( aMesh.tangents );
            colors = CreateList( aMesh.colors32 );
            boneWeights = CreateList( aMesh.boneWeights );
        }
        
        private List<T> CreateList<T>( T[] aSource )
        {
            if ( aSource == null || aSource.Length == 0 )
                return null;
            return new List<T>( aSource );
        }
        private void Copy<T>( ref List<T> aDest, List<T> aSource, int aIndex )
        {
            if ( aSource == null )
                return;
            if ( aDest == null )
                aDest = new List<T>();
            aDest.Add( aSource[aIndex] );
        }
        public int Add( Vertices aOther, int aIndex )
        {
            int i = verts.Count;
            Copy( ref verts, aOther.verts, aIndex );
            Copy( ref uv1, aOther.uv1, aIndex );
            Copy( ref uv2, aOther.uv2, aIndex );
            Copy( ref uv3, aOther.uv3, aIndex );
            Copy( ref uv4, aOther.uv4, aIndex );
            Copy( ref normals, aOther.normals, aIndex );
            Copy( ref tangents, aOther.tangents, aIndex );
            Copy( ref colors, aOther.colors, aIndex );
            Copy( ref boneWeights, aOther.boneWeights, aIndex );
            return i;
        }
        public void AssignTo( Mesh aTarget )
        {
            aTarget.SetVertices( verts );
            if ( uv1 != null ) aTarget.SetUVs( 0, uv1 );
            if ( uv2 != null ) aTarget.SetUVs( 1, uv2 );
            if ( uv3 != null ) aTarget.SetUVs( 2, uv3 );
            if ( uv4 != null ) aTarget.SetUVs( 3, uv4 );
            if ( normals != null ) aTarget.SetNormals( normals );
            if ( tangents != null ) aTarget.SetTangents( tangents );
            if ( colors != null ) aTarget.SetColors( colors );
            if ( boneWeights != null ) aTarget.boneWeights = boneWeights.ToArray();
        }
    }
    
    public static Mesh GetSubmesh( this Mesh aMesh, int aSubMeshIndex )
    {
        if ( aSubMeshIndex < 0 || aSubMeshIndex >= aMesh.subMeshCount )
            return null;
        int[] indices = aMesh.GetTriangles( aSubMeshIndex );
        Vertices source = new Vertices( aMesh );
        Vertices dest = new Vertices();
        Dictionary<int, int> map = new Dictionary<int, int>();
        int[] newIndices = new int[indices.Length];
        for ( int i = 0; i < indices.Length; i++ )
        {
            int o = indices[i];
            int n;
            if ( !map.TryGetValue( o, out n ) )
            {
                n = dest.Add( source, o );
                map.Add( o, n );
            }
            newIndices[i] = n;
        }
        Mesh m = new Mesh();
        dest.AssignTo( m );
        m.triangles = newIndices;
        return m;
    }
}

#endif