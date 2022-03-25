using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIGrid
{
    public Cell[,] grid;

    List<Tile> allTiles;
    Vector2Int gridSize;

    public FIGrid(Vector2Int gridSize, List<Tile> allTiles)
    {
        this.gridSize = gridSize;
        this.allTiles = allTiles;
        InitialiseGrid();
    }

    private void InitialiseGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];

        for (int col = 0; col < gridSize.x; col++)
        {
            for (int row = 0; row < gridSize.y; row++)
            {
                grid[col, row] = new Cell(new List<Tile>(allTiles), new Vector2Int(col, row));
            }
        }
    }


    /// <summary>
    /// returns true if all cells in the grid have only 1 possible tile. Else returns false.
    /// </summary>
    /// <returns></returns>
    public bool WaveFunctionCollapsed()
    {
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[col, row].possibleTiles.Count > 1)
                {
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Returns the coordinates of the cell with the fewest possible tiles above 1.
    /// </summary>
    public Vector2Int GetLowestEntropy()
    {
        Vector2Int lowestIndex = Vector2Int.zero;

        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[col, row].possibleTiles.Count > 1)
                {
                    // todo return coords if the value is two else do this? That might be quicker because 2 is the lowest value needed for returning.
                    if (grid[col, row].possibleTiles.Count < grid[lowestIndex.x, lowestIndex.y].possibleTiles.Count || grid[lowestIndex.x, lowestIndex.y].possibleTiles.Count <= 1)
                    {
                        lowestIndex.x = col;
                        lowestIndex.y = row;
                    }
                }
            }
        }

        return lowestIndex;
    }


    /// <summary>
    /// returns the coordinates for the N/E/S/W neighbours.
    /// If the neighbour is invalid, returns Vector2(-1, -1)
    /// </summary>
    public Vector2Int[] GetNeighbourCoords(Vector2Int coords)
    {
        Vector2Int[] Neighbours = new Vector2Int[4];

        #region Get the coodinates

        if (coords.y + 1 < gridSize.y)
        {
            Neighbours[0] = new Vector2Int(coords.x, coords.y + 1);
        }
        else
        {
            Neighbours[0] = new Vector2Int(-1, -1);
        }

        if (coords.x + 1 < gridSize.x)
        {
            Neighbours[1] = (new Vector2Int(coords.x + 1, coords.y));
        }
        else
        {
            Neighbours[1] = new Vector2Int(-1, -1);
        }

        if (coords.y - 1 >= 0)
        {
            Neighbours[2] = (new Vector2Int(coords.x, coords.y - 1));
        }
        else
        {
            Neighbours[2] = new Vector2Int(-1, -1);
        }

        if (coords.x - 1 >= 0)
        {
            Neighbours[3] = (new Vector2Int(coords.x - 1, coords.y));
        }
        else
        {
            Neighbours[3] = new Vector2Int(-1, -1);
        }

        #endregion

        return Neighbours;
    }


    /// <summary>
    /// Returns an array of the N/E/S/W neighbours of the given cell.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public Cell[] GetNeighbouringCells(Cell cell)
    {
        Cell[] neighbours = new Cell[4];

        #region Get the coodinates

        if (cell.gridCoords.y + 1 < gridSize.y)
        {
            neighbours[0] = grid[cell.gridCoords.x, cell.gridCoords.y + 1];
        }
        else
        {
            neighbours[0] = null;
        }

        if (cell.gridCoords.x + 1 < gridSize.x)
        {
            neighbours[1] = (grid[cell.gridCoords.x + 1, cell.gridCoords.y]);
        }
        else
        {
            neighbours[1] = null;
        }

        if (cell.gridCoords.y - 1 >= 0)
        {
            neighbours[2] = (grid[cell.gridCoords.x, cell.gridCoords.y - 1]);
        }
        else
        {
            neighbours[2] = null;
        }

        if (cell.gridCoords.x - 1 >= 0)
        {
            neighbours[3] = (grid[cell.gridCoords.x - 1, cell.gridCoords.y]);
        }
        else
        {
            neighbours[3] = null;
        }

        #endregion

        return neighbours;
    }


    public Cell GetCell(Vector2Int coords)
    {
        if (coords.x < gridSize.x && coords.x >= 0)
        {
            if (coords.y < gridSize.y && coords.y >= 0)
            {
                return grid[coords.x, coords.y];
            }
        }

        return null;
    }

    public Cell GetCell(int x, int y)
    {
        if (x < gridSize.x && x >= 0)
        {
            if (y < gridSize.y && y >= 0)
            {
                return grid[x, y];
            }
        }

        return null;
    }


}
