using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    public static MarchingCubes instance = null;
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


    /// <summary>
    /// Creates the vertices for the grid
    /// </summary>
    private void InitialiseVertices()
    {
        vertices = new MCVertex[gridSize.x + 1, gridSize.y + 1, gridSize.z + 1];

        for (int x = 0; x < vertices.GetLength(0); x++)
        {
            for (int y = 0; y < vertices.GetLength(1); y++)
            {
                for (int z = 0; z < vertices.GetLength(2); z++)
                {
                    vertices[x, y, z] = new MCVertex();
                    vertices[x, y, z].coords = new Vector3Int(x, y, z);
                }
            }
        }
        //for (int row = 0; row < vertices.GetLength(0); row++)
        //{
        //    for (int col = 0; col < vertices.GetLength(1); col++)
        //    {
        //        for (int pillar = 0; pillar < vertices.GetLength(2); pillar++)
        //        {
        //            vertices[col, row, pillar] = new MCVertex();
        //            vertices[col, row, pillar].coords = new Vector3Int(col, row, pillar);
        //        }
        //    }
        //}
    }


    /// <summary>
    /// Creates the cells for the grid. Each cell has 8 vertices
    /// </summary>
    private void InitialiseCells()
    {
        cells = new MCCell[gridSize.x, gridSize.y, gridSize.z];

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int z = 0; z < cells.GetLength(2); z++)
                {
                    cells[x, y, z] = new MCCell();
                    cells[x, y, z].coords = new Vector3Int(x, y, z);

                    MCVertex[] verts = new MCVertex[8];
                    // bottom vertices
                    verts[0] = vertices[x, y, z];
                    verts[1] = vertices[x, y, z + 1];
                    verts[2] = vertices[x + 1, y, z + 1];
                    verts[3] = vertices[x + 1, y, z];

                    // top verticies
                    verts[4] = vertices[x, y + 1, z];
                    verts[5] = vertices[x, y + 1, z + 1];
                    verts[6] = vertices[x + 1, y + 1, z + 1];
                    verts[7] = vertices[x + 1, y + 1, z];

                    cells[x, y, z].vertices = verts;
                }
            }
        }

        //for (int row = 0; row < cells.GetLength(0); row++)
        //{
        //    for (int col = 0; col < cells.GetLength(1); col++)
        //    {
        //        for (int pillar = 0; pillar < cells.GetLength(2); pillar++)
        //        {
        //            cells[col, row, pillar] = new MCCell();
        //            cells[col, row, pillar].coords = new Vector3Int(col, row, pillar);

        //            MCVertex[] verts = new MCVertex[8];
        //            // bottom vertices
        //            verts[0] = vertices[col, row, pillar];
        //            verts[1] = vertices[col, row + 1, pillar];
        //            verts[2] = vertices[col + 1, row + 1, pillar];
        //            verts[3] = vertices[col + 1, row, pillar];

        //            // top verticies
        //            verts[4] = vertices[col, row, pillar + 1];
        //            verts[5] = vertices[col, row + 1, pillar + 1];
        //            verts[6] = vertices[col + 1, row + 1, pillar + 1];
        //            verts[7] = vertices[col + 1, row, pillar + 1];

        //            cells[col, row, pillar].vertices = verts;
        //        }
        //    }
        //}
    }




    /// <summary>
    /// Sets the desired vertex to true and updates the cells.
    /// </summary>
    /// <param name="coords"></param>
    public void FillVertex(Vector3Int coords)
    {
        vertices[coords.x, coords.y, coords.z].full = true;
        UpdateCells();
    }


    /// <summary>
    /// Sets the desired vertex to false and updates the cells.
    /// </summary>
    /// <param name="coords"></param>
    public void ClearVertex(Vector3Int coords)
    {
        vertices[coords.x, coords.y, coords.z].full = false;
        UpdateCells();
    }


    /// <summary>
    /// Goes through each cell and calculates which tiles it will show.
    /// </summary>
    private void UpdateCells()
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int z = 0; z < cells.GetLength(2); z++)
                {
                    cells[x, y, z].Update(prefabs);
                }
            }
        }
        // for (int row = 0; row < cells.GetLength(0); row++)
        //    {
        //        for (int col = 0; col < cells.GetLength(1); col++)
        //        {
        //            for (int pillar = 0; pillar < cells.GetLength(2); pillar++)
        //            {
        //                cells[col, row, pillar].Update(prefabs);
        //            }
        //        }
        //    }
    }
}