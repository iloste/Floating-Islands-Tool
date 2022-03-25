using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCGrid
{
    public Vector3Int gridSize;
    public MCCell[,,] grid;
    List<MCTile> allTiles;

    public MCGrid(Vector3Int gridSize, List<MCTile> allTiles)
    {
        this.gridSize = gridSize;
        this.allTiles = allTiles;
        InitialiseGrid();
    }


    private void InitialiseGrid()
    {
        grid = new MCCell[gridSize.x, gridSize.y, gridSize.z];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    grid[x, y, z] = new MCCell(new List<MCTile>(allTiles), new Vector3Int(x, y, z));
                }
            }
        }
    }


    /// <summary>
    /// returns true if all cells in the grid have only 1 possible tile. Else returns false.
    /// </summary>
    /// <returns></returns>
    public bool WaveFunctionCollapsed()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z].possibleTiles.Count > 1)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Returns the coordinates of the cell with the fewest possible tiles above 1.
    /// </summary>
    public Vector3Int GetLowestEntropy()
    {
        Vector3Int lowestIndex = Vector3Int.zero;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z].possibleTiles.Count > 1)
                    {
                        // todo return coords if the value is two else do this? That might be quicker because 2 is the lowest value needed for returning.
                        if (grid[x, y, z].possibleTiles.Count < grid[lowestIndex.x, lowestIndex.y, lowestIndex.z].possibleTiles.Count ||
                            grid[lowestIndex.x, lowestIndex.y, lowestIndex.z].possibleTiles.Count <= 1)
                        {
                            lowestIndex.x = x;
                            lowestIndex.y = y;
                            lowestIndex.z = z;
                        }
                    }
                }
            }
        }

        return lowestIndex;
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
                    grid[x, y, z].ResetPossibleTiles();
                }
            }
        }
    }
}
