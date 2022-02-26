using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralMesh : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Mesh mesh;
    [SerializeField] VertexList vertexList;
    [SerializeField] Material material;

    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        GenerateMesh();
    }

    protected void GenerateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertexList.vertexPositions;
        int[] triangles = new int[vertexList.triplets.Length * 3];

        for (int i = 0; i < vertexList.triplets.Length; i++)
        {
            triangles[(i * 3)] = vertexList.triplets[i].x;
            triangles[(i * 3) + 1] = vertexList.triplets[i].y;
            triangles[(i * 3) + 2] = vertexList.triplets[i].z;

        }

        mesh.triangles = triangles;
        GetComponent<MeshRenderer>().material = material;
        //Vector2[] uvs = new Vector2[3];
        //uvs[0] = new Vector2(0, 1); //top-left
        //uvs[1] = new Vector2(1, 1); //top-right
        //uvs[2] = new Vector2(0, 0); //bottom-left
        //                            // uvs[3] = new Vector2(1, 0); //bottom-right

        mesh.uv = vertexList.uvs;
    }

}
