using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleClickTest : MonoBehaviour
{
    [SerializeField] VertexList vertexList;
    MeshFilter meshFilter;
    Mesh mesh;

    Vector3 worldPosRaw;
    Vector3Int worldPosRefined;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vertexList.Initialise();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetWorldPositions();

            // get vertices
            Vector3[] vertices = GetVertices();
            bool inBottomTri = isInside(vertices[0].x, vertices[0].z, vertices[1].x, vertices[1].z, vertices[2].x, vertices[2].z, worldPosRaw.x, worldPosRaw.z);
            Vector3Int triplet = GetTriplet(inBottomTri, vertices);
            vertexList.AddTriplet(triplet);
            GenerateMesh();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GetWorldPositions();

            // get vertices
            Vector3[] vertices = GetVertices();
            bool inBottomTri = isInside(vertices[0].x, vertices[0].z, vertices[1].x, vertices[1].z, vertices[2].x, vertices[2].z, worldPosRaw.x, worldPosRaw.z);
            Vector3Int triplet = GetTriplet(inBottomTri, vertices);
            vertexList.RemoveTriplet(triplet);
            GenerateMesh();
        }
    }

    private void GenerateMesh()
    {
        mesh.vertices = vertexList.vertexPositions;
        int[] triangles = new int[vertexList.triplets.Length * 3];

        for (int i = 0; i < vertexList.triplets.Length; i++)
        {
            triangles[(i * 3)] = vertexList.triplets[i].x;
            triangles[(i * 3) + 1] = vertexList.triplets[i].y;
            triangles[(i * 3) + 2] = vertexList.triplets[i].z;
        }

        mesh.triangles = triangles;
    }

    private void GetWorldPositions()
    {
        worldPosRaw = new Vector3();
        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            worldPosRaw = ray.GetPoint(distance);
        }

        worldPosRefined = new Vector3Int((int)worldPosRaw.x, 0, (int)worldPosRaw.z);
    }

    private Vector3[] GetVertices()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = worldPosRefined;
        vertices[1] = worldPosRefined + new Vector3(0, 0, 1);
        vertices[2] = worldPosRefined + new Vector3(1, 0, 0);
        vertices[3] = worldPosRefined + new Vector3(1, 0, 1);

        return vertices;
    }

    private Vector3Int GetTriplet(bool inBottomTri, Vector3[] vertices)
    {
        Vector3Int triplet = Vector3Int.zero;

        if (inBottomTri)
        {
            triplet.x = vertexList.AddVertexPosition(vertices[0]);
            triplet.y = vertexList.AddVertexPosition(vertices[1]);
            triplet.z = vertexList.AddVertexPosition(vertices[2]);
        }
        else
        {
            triplet.x = vertexList.AddVertexPosition(vertices[1]);
            triplet.y = vertexList.AddVertexPosition(vertices[3]);
            triplet.z = vertexList.AddVertexPosition(vertices[2]);
        }

        return triplet;
    }

    /* A utility function to calculate area of triangle
   formed by (x1, y1) (x2, y2) and (x3, y3) */
    static double area(float x1, float y1, float x2,
                       float y2, float x3, float y3)
    {
        return System.Math.Abs((x1 * (y2 - y3) +
                         x2 * (y3 - y1) +
                         x3 * (y1 - y2)) / 2.0);
    }

    /* A function to check whether pofloat P(x, y) lies
    inside the triangle formed by A(x1, y1),
    B(x2, y2) and C(x3, y3) */
    static bool isInside(float x1, float y1, float x2,
                         float y2, float x3, float y3,
                         float x, float y)
    {
        /* Calculate area of triangle ABC */
        double A = area(x1, y1, x2, y2, x3, y3);

        /* Calculate area of triangle PBC */
        double A1 = area(x, y, x2, y2, x3, y3);

        /* Calculate area of triangle PAC */
        double A2 = area(x1, y1, x, y, x3, y3);

        /* Calculate area of triangle PAB */
        double A3 = area(x1, y1, x2, y2, x, y);

        /* Check if sum of A1, A2 and A3 is same as A */
        return (A == A1 + A2 + A3);
    }

}
