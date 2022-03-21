using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubesTest : MonoBehaviour
{
    public static MarchingCubesTest instance = null;
    [SerializeField] GameObject[] prefabs;

    [SerializeField] Vector3Int gridSize;
    [SerializeField] GameObject sg;

    MCVertex[,,] vertices;
    MCCell[,,] cells;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        InitialiseVertices();
        InitialiseCells();
    }


    private void InitialiseVertices()
    {
        vertices = new MCVertex[gridSize.x + 1, gridSize.y + 1, gridSize.z + 1];

        for (int row = 0; row < vertices.GetLength(0); row++)
        {
            for (int col = 0; col < vertices.GetLength(1); col++)
            {
                for (int pillar = 0; pillar < vertices.GetLength(2); pillar++)
                {
                    vertices[col, row, pillar] = new MCVertex();
                    vertices[col, row, pillar].coords = new Vector3Int(col, row, pillar);
                }
            }
        }
    }


    private void InitialiseCells()
    {
        cells = new MCCell[gridSize.x, gridSize.y, gridSize.z];

        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                for (int pillar = 0; pillar < cells.GetLength(2); pillar++)
                {
                    cells[col, row, pillar] = new MCCell();
                    cells[col, row, pillar].coords = new Vector3Int(col, row, pillar);

                    MCVertex[] verts = new MCVertex[8];
                    // bottom vertices
                    verts[0] = vertices[col, row, pillar];
                    verts[1] = vertices[col, row + 1, pillar];
                    verts[2] = vertices[col + 1, row + 1, pillar];
                    verts[3] = vertices[col + 1, row, pillar];

                    // top verticies
                    verts[4] = vertices[col, row, pillar + 1];
                    verts[5] = vertices[col, row + 1, pillar + 1];
                    verts[6] = vertices[col + 1, row + 1, pillar + 1];
                    verts[7] = vertices[col + 1, row, pillar + 1];

                    cells[col, row, pillar].vertices = verts;

                    // create a new vertex and pass that in, see if that works.
                    // Then try storing it as it's own variable, not an array.


                    //cells[col, row].vertices[0] = vertices[col, row];
                    //cells[col, row].vertices[1] = vertices[col + 1, row];
                    //cells[col, row].vertices[2] = vertices[col + 1, row + 1];
                    //cells[col, row].vertices[3] = vertices[col, row + 1];


                }

            }
        }
    }





    public void FillVertex(Vector3Int coords)
    {
        vertices[coords.x, coords.y, coords.z].full = true;
        // Instantiate(sg, new Vector3(coords.x, 0, coords.y), Quaternion.identity);

        UpdateCells();
    }


    private void UpdateCells()
    {
        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                for (int pillar = 0; pillar < cells.GetLength(2); pillar++)
                {
                    cells[col, row, pillar].Update(prefabs);
                }
            }
        }
    }
}