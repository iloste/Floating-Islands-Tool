using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCVertexGrid
{
    public MCVertex[,,] grid;
    Vector3Int gridSize;
    GameObject debugParent;
    GameObject debugVertexPrefab;

    public MCVertexGrid(Vector3Int gridSize, GameObject prefab)
    {
        this.gridSize = gridSize + new Vector3Int(1, 1, 1);
        this.debugVertexPrefab = prefab;
        // create parent game object to hold debug objects
        debugParent = GameObject.Instantiate(new GameObject());
        debugParent.name = "Vertex Grid";

        InitialiseGrid();
    }


    private void InitialiseGrid()
    {
        grid = new MCVertex[gridSize.x + 1, gridSize.y + 1, gridSize.z + 1];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    grid[x, y, z] = new MCVertex();
                    grid[x, y, z].coords = new Vector3Int(x, y, z);

                    // place icosphere in vertex's place for debug purposes
                    grid[x, y, z].vertexGO = GameObject.Instantiate(debugVertexPrefab, new Vector3(x, y, z), Quaternion.identity, debugParent.transform);
                    grid[x, y, z].vertexGO.name = "" + x + ", " + y + ", " + z;
                    grid[x, y, z].vertexGO.transform.position += new Vector3(-.1f, 0, -0);
                    grid[x, y, z].vertexGO.transform.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }


    public void DisplayVertices(bool display)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    grid[x, y, z].vertexGO.SetActive(display);
                }
            }
        }
    }


    public void FillVertex(Vector3Int coords)
    {
        grid[coords.x, coords.y, coords.z].full = true;
        grid[coords.x, coords.y, coords.z].vertexGO.transform.GetComponent<Renderer>().material.color = Color.green;
    }


    public void ClearVertex(Vector3Int coords)
    {
        grid[coords.x, coords.y, coords.z].full = false;
        grid[coords.x, coords.y, coords.z].vertexGO.transform.GetComponent<Renderer>().material.color = Color.red;
    }
}
