using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleGridTest : MonoBehaviour
{
    [SerializeField] bool addUnderneath = true;



    enum Side
    {
        Top,
        Bottom,
        Left,
        Right,
    }


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
        UserInput();
    }




    private void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetWorldPositions();

            Vector3[] vertices = GetVertices();
            Side side = GetSide(worldPosRaw, vertices);
            Vector3Int triplet = GetTriplet(side, vertices);
            vertexList.AddTriplet(triplet);

            AddUnderneath(vertices, side);



            GenerateMesh();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GetWorldPositions();

            Vector3[] vertices = GetVertices();
            Side side = GetSide(worldPosRaw, vertices);
            Vector3Int triplet = GetTriplet(side, vertices);
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

        float x = (int)worldPosRaw.x;
        float y = (int)worldPosRaw.y;
        float z = (int)worldPosRaw.z;

        if (worldPosRaw.x - x > 0.5)
        {
            x += 0.5f;
        }
        if (worldPosRaw.x - y > 0.5)
        {
            y += 0.5f;
        }
        if (worldPosRaw.x - z > 0.5)
        {
            z += 0.5f;
        }
    }


    private Vector3[] GetVertices()
    {
        Vector3[] vertices = new Vector3[5];
        vertices[0] = worldPosRefined;
        vertices[1] = worldPosRefined + new Vector3(0, 0, 1);
        vertices[2] = worldPosRefined + new Vector3(1, 0, 0);
        vertices[3] = worldPosRefined + new Vector3(1, 0, 1);
        vertices[4] = worldPosRefined + new Vector3(0.5f, 0, 0.5f);

        return vertices;
    }

    private void AddUnderneath(Vector3[] topVertices, Side side)
    {
        Vector3[] vertices = new Vector3[4];
        switch (side)
        {
            case Side.Top:
                vertices[0] = topVertices[1];
                vertices[1] = topVertices[3];
                vertices[2] = topVertices[4];
                break;
            case Side.Bottom:
                vertices[0] = topVertices[0];
                vertices[1] = topVertices[4];
                vertices[2] = topVertices[2];
                break;
            case Side.Left:
                vertices[0] = topVertices[1];
                vertices[1] = topVertices[4];
                vertices[2] = topVertices[0];
                break;
            case Side.Right:
                vertices[0] = topVertices[4];
                vertices[1] = topVertices[3];
                vertices[2] = topVertices[2];
                break;
            default:
                break;
        }

        

        vertices[3].x = (vertices[0].x + vertices[1].x + vertices[2].x) / 3;
        vertices[3].z = (vertices[0].z + vertices[1].z + vertices[2].z) / 3;
        vertices[3].y = -0.6f;
                                     
        Vector3Int triplet1 = Vector3Int.zero;
        Vector3Int triplet2 = Vector3Int.zero;
        Vector3Int triplet3 = Vector3Int.zero;

        triplet1.x = vertexList.AddVertex(vertices[1]);
        triplet1.y = vertexList.AddVertex(vertices[0]);
        triplet1.z = vertexList.AddVertex(vertices[3]);
        vertexList.AddTriplet(triplet1);

        triplet2.x = vertexList.AddVertex(vertices[2]);
        triplet2.y = vertexList.AddVertex(vertices[1]);
        triplet2.z = vertexList.AddVertex(vertices[3]);
        vertexList.AddTriplet(triplet2);

        triplet3.x = vertexList.AddVertex(vertices[0]);
        triplet3.y = vertexList.AddVertex(vertices[2]);
        triplet3.z = vertexList.AddVertex(vertices[3]);
        vertexList.AddTriplet(triplet3);
    }





    private Vector3Int GetTriplet(Side side, Vector3[] vertices)
    {
        Vector3Int triplet = Vector3Int.zero;
        switch (side)
        {
            case Side.Top:
                triplet.x = vertexList.AddVertex(vertices[1]);
                triplet.y = vertexList.AddVertex(vertices[3]);
                triplet.z = vertexList.AddVertex(vertices[4]);
                break;
            case Side.Bottom:
                triplet.x = vertexList.AddVertex(vertices[0]);
                triplet.y = vertexList.AddVertex(vertices[4]);
                triplet.z = vertexList.AddVertex(vertices[2]);
                break;
            case Side.Left:
                triplet.x = vertexList.AddVertex(vertices[0]);
                triplet.y = vertexList.AddVertex(vertices[1]);
                triplet.z = vertexList.AddVertex(vertices[4]);
                break;
            case Side.Right:
                triplet.x = vertexList.AddVertex(vertices[4]);
                triplet.y = vertexList.AddVertex(vertices[3]);
                triplet.z = vertexList.AddVertex(vertices[2]);
                break;
            default:
                break;
        }

        return triplet;
    }


    private Side GetSide(Vector3 pos, Vector3[] vertices)
    {
        bool inBottomTri = isInside(vertices[0].x, vertices[0].z, vertices[1].x, vertices[1].z, vertices[2].x, vertices[2].z, worldPosRaw.x, worldPosRaw.z);

        if (inBottomTri)
        {
            inBottomTri = isInside(vertices[0].x, vertices[0].z, vertices[4].x, vertices[4].z, vertices[2].x, vertices[2].z, worldPosRaw.x, worldPosRaw.z);

            if (inBottomTri)
            {
                return Side.Bottom;
            }
            else
            {
                return Side.Left;
            }
        }
        else
        {
            inBottomTri = isInside(vertices[4].x, vertices[4].z, vertices[3].x, vertices[3].z, vertices[2].x, vertices[2].z, worldPosRaw.x, worldPosRaw.z);

            if (inBottomTri)
            {
                return Side.Right;
            }
            else
            {
                return Side.Top;
            }
        }
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
