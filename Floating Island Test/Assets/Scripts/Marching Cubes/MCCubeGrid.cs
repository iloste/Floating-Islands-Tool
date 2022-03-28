using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCubeGrid
{
    public MCCube[,,] grid;
    Vector3Int gridSize;
    GameObject debugPrefab;

    public MCCubeGrid(Vector3Int gridSize, MCVertexGrid vertexGrid, MCCellGrid cellGrid, GameObject debugPrefab)
    {
        this.gridSize = gridSize;
        this.debugPrefab = debugPrefab;
        InitialiseGrid(vertexGrid, cellGrid);
    }


    private void InitialiseGrid(MCVertexGrid vertexGrid, MCCellGrid cellGrid)
    {
        grid = new MCCube[gridSize.x, gridSize.y, gridSize.z];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z] = new MCCube();
                    grid[x, y, z].coords = new Vector3Int(x, y, z);

                    #region Get Vertices
                    MCVertex[] verts = new MCVertex[8];
                    // bottom vertices
                    verts[0] = vertexGrid.grid[x, y, z];
                    verts[1] = vertexGrid.grid[x, y, z + 1];
                    verts[2] = vertexGrid.grid[x + 1, y, z + 1];
                    verts[3] = vertexGrid.grid[x + 1, y, z];

                    // top verticies
                    verts[4] = vertexGrid.grid[x, y + 1, z];
                    verts[5] = vertexGrid.grid[x, y + 1, z + 1];
                    verts[6] = vertexGrid.grid[x + 1, y + 1, z + 1];
                    verts[7] = vertexGrid.grid[x + 1, y + 1, z];

                    grid[x, y, z].vertices = verts;
                    #endregion

                    #region Get Cells
                    MCCell[] cells = new MCCell[8];
                    // bottom vertices
                    cells[0] = cellGrid.GetCell(x * 2, y * 2, z * 2);
                    cells[1] = cellGrid.GetCell(x * 2, y * 2, z * 2 + 1);
                    cells[2] = cellGrid.GetCell(x * 2 + 1, y * 2, z * 2 + 1);
                    cells[3] = cellGrid.GetCell(x * 2 + 1, y * 2, z * 2);

                    // top verticies
                    cells[4] = cellGrid.GetCell(x * 2, y * 2 + 1, z * 2);
                    cells[5] = cellGrid.GetCell(x * 2, y * 2 + 1, z * 2 + 1);
                    cells[6] = cellGrid.GetCell(x * 2 + 1, y * 2 + 1, z * 2 + 1);
                    cells[7] = cellGrid.GetCell(x * 2 + 1, y * 2 + 1, z * 2);

                    grid[x, y, z].cells = cells;
                    #endregion
                }
            }
        }
    }


    public void UpdateCubesCells()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z].UpdateCubesCells();
                }
            }
        }
    }

    public void DisplayCubes()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z].Update2();
                }
            }
        }
    }

    private void DisplayCubeLocations()
    {
        GameObject cubeGrid = GameObject. Instantiate(new GameObject());
        cubeGrid.name = "cube Grid";

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    GameObject temp = GameObject.Instantiate(debugPrefab, new Vector3(x, y, z), Quaternion.identity, cubeGrid.transform);
                    temp.name = "" + x + ", " + y + ", " + z;
                    temp.transform.position += new Vector3(0.5f, 0.5f, 0.5f);
                }
            }
        }
    }
}
