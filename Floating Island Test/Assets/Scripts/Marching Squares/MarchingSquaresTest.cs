using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquaresTest : MonoBehaviour
{
    public static MarchingSquaresTest instance = null;
    [SerializeField] GameObject[] prefabs;

    [SerializeField] Vector2Int gridSize;
    [SerializeField] GameObject sg;
    
    MSVertex[,] vertices;
    MSCell[,] cells;
    
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


    private void InitialiseCells()
    {
        cells = new MSCell[gridSize.x, gridSize.y];

        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                cells[col, row] = new MSCell();
                cells[col, row].coords = new Vector2Int(col, row);

                MSVertex[] verts = new MSVertex[4];
                verts[0] = vertices[col, row];
                verts[1] = vertices[col, row + 1];
                verts[2] = vertices[col + 1, row + 1];
                verts[3] = vertices[col + 1, row];

                cells[col, row].vertices = verts;

                // create a new vertex and pass that in, see if that works.
                // Then try storing it as it's own variable, not an array.


                //cells[col, row].vertices[0] = vertices[col, row];
                //cells[col, row].vertices[1] = vertices[col + 1, row];
                //cells[col, row].vertices[2] = vertices[col + 1, row + 1];
                //cells[col, row].vertices[3] = vertices[col, row + 1];



            }
        }
    }


    private void InitialiseVertices()
    {
        vertices = new MSVertex[gridSize.x + 1, gridSize.y + 1];

        for (int row = 0; row < vertices.GetLength(0); row++)
        {
            for (int col = 0; col < vertices.GetLength(1); col++)
            {
                vertices[col, row] = new MSVertex();
                vertices[col, row].coords = new Vector2Int(col, row);
            }
        }
    }


    public void FillVertex(Vector2Int coords)
    {
        vertices[coords.x, coords.y].full = true;
       // Instantiate(sg, new Vector3(coords.x, 0, coords.y), Quaternion.identity);

         UpdateCells();
    }


    private void UpdateCells()
    {
        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                cells[col, row].Update(prefabs);
            }
        }
    }
}
