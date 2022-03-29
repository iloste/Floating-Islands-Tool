using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCellGrid
{
    public Vector3Int gridSize;
    public MCCell[,,] grid;
    List<MCTile> allTiles;

    GameObject debugPrefab;

    public MCCellGrid(Vector3Int gridSize, List<MCTile> allTiles)
    {
        this.gridSize = gridSize * 2;
        this.allTiles = allTiles;
        InitialiseGrid();
    }

    public MCCellGrid(Vector3Int gridSize, List<MCTile> allTiles, GameObject debugPrefab)
    {
        this.debugPrefab = debugPrefab;

        this.gridSize = gridSize * 2;
        this.allTiles = allTiles;
        InitialiseGrid();
    }


    private void InitialiseGrid()
    {
        GameObject cellGrid = GameObject.Instantiate(new GameObject());
        cellGrid.name = "cell Grid";

        grid = new MCCell[gridSize.x, gridSize.y, gridSize.z];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z] = new MCCell(new List<MCTile>(allTiles), new Vector3Int(x, y, z));
                    grid[x, y, z].debugGO = GameObject.Instantiate(debugPrefab, grid[x, y, z].worldPosition, Quaternion.identity, cellGrid.transform);
                    grid[x, y, z].debugGO.name = "" + x + ", " + y + ", " + z;
                    grid[x, y, z].debugGO.transform.position += new Vector3(0.25f, 0.25f, 0.25f);
                    grid[x, y, z].SetTileExists(false);
                }
            }
        }
    }


    /// <summary>
    /// returns true if all active cells in the grid have only 1 possible tile. Else returns false.
    /// </summary>
    /// <returns></returns>
    public bool GridCollapsed()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z].TileExists)
                    {
                        if (grid[x, y, z].possibleTiles.Count > 1)
                        {
                            return false;
                        } 
                    }
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Returns the coordinates of the cell with the fewest possible tiles above 1.
    /// </summary>
    public MCCell GetLowestEntropy()
    {
        Vector3Int lowestIndex = Vector3Int.zero;
        MCCell lowestEntropy = null;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z].TileExists)
                    {
                        if (grid[x, y, z].possibleTiles.Count > 1)
                        {
                            if (lowestEntropy == null || grid[x, y, z].possibleTiles.Count < lowestEntropy.possibleTiles.Count)
                            {
                                lowestEntropy = grid[x, y, z];
                            }

                            // can't be lower than 2 without being collapsed
                            if (lowestEntropy.possibleTiles.Count == 2)
                            {
                                return lowestEntropy;
                            }
                        }
                    }
                }
            }
        }

        return lowestEntropy;
    }

    /// <summary>
    /// Returns an array of the N/E/S/W neighbours of the given cell.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public MCCell[] GetNeighbouringCells(MCCell cell)
    {
        MCCell[] neighbours = new MCCell[6];

        #region Get the coodinates
        //north
        if (cell.coords.y + 1 < gridSize.y && grid[cell.coords.x, cell.coords.y, cell.coords.z + 1].TileExists)
        {
            neighbours[0] = grid[cell.coords.x, cell.coords.y, cell.coords.z + 1];
        }
        else
        {
            neighbours[0] = null;
        }
        //east
        if (cell.coords.x + 1 < gridSize.x && grid[cell.coords.x + 1, cell.coords.y, cell.coords.z].TileExists)
        {
            neighbours[1] = (grid[cell.coords.x + 1, cell.coords.y, cell.coords.z]);
        }
        else
        {
            neighbours[1] = null;
        }

        //south
        if (cell.coords.y - 1 >= 0 && grid[cell.coords.x, cell.coords.y, cell.coords.z - 1].TileExists)
        {
            neighbours[2] = (grid[cell.coords.x, cell.coords.y, cell.coords.z - 1]);
        }
        else
        {
            neighbours[2] = null;
        }

        //west
        if (cell.coords.x - 1 >= 0 && grid[cell.coords.x - 1, cell.coords.y, cell.coords.z].TileExists)
        {
            neighbours[3] = (grid[cell.coords.x - 1, cell.coords.y, cell.coords.z]);
        }
        else
        {
            neighbours[3] = null;
        }

        //up
        if (cell.coords.z + 1 < gridSize.z && grid[cell.coords.x, cell.coords.y + 1, cell.coords.z].TileExists)
        {
            neighbours[4] = (grid[cell.coords.x, cell.coords.y + 1, cell.coords.z]);
        }
        else
        {
            neighbours[4] = null;
        }

        //down
        if (cell.coords.z - 1 >= 0 && grid[cell.coords.x, cell.coords.y - 1, cell.coords.z].TileExists)
        {
            neighbours[5] = (grid[cell.coords.x, cell.coords.y - 1, cell.coords.z]);
        }
        else
        {
            neighbours[5] = null;
        }

        #endregion

        return neighbours;
    }

    public MCCell GetCell(Vector3Int coords)
    {
        if (coords.x < gridSize.x && coords.x >= 0)
        {
            if (coords.y < gridSize.y && coords.y >= 0)
            {
                if (coords.z < gridSize.z && coords.z >= 0)
                {
                    return grid[coords.x, coords.y, coords.z];
                }
            }
        }

        return null;
    }


    public MCCell GetCell(int x, int y, int z)
    {
        if (x < gridSize.x && x >= 0)
        {
            if (y < gridSize.y && y >= 0)
            {
                if (z < gridSize.z && z >= 0)
                {
                    return grid[x, y, z];
                }
            }
        }

        return null;
    }


    public void ResetPossibilitySpace()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (x == 1 && y == 1 && z == 1)
                    {

                    }
                    if (x == 3 && y == 1 && z == 2)
                    {

                    }
                    grid[x, y, z].ResetPossiblitySpace();
                    //grid[x, y, z].ResetPossibleTiles(allTiles);
                }
            }
        }
    }
}
