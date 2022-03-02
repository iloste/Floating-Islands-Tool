using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapse2D : MonoBehaviour
{
    [SerializeField] Tile[] originalTiles;
    [SerializeField] Vector2Int gridSize;
    List<Tile> allTiles;

    //List<List<Cell>> grid;
    [SerializeField] bool debug;
    [SerializeField] bool randomSeed;
    [SerializeField] int seed;
    FIGrid grid;

    int oppositeDirModifier = 2;

    private void Awake()
    {
        if (randomSeed)
        {
            seed = Random.Range(0, 300000);
            Debug.Log(seed);
        }

        Random.InitState(seed);

        InitialiseTiles();
        InitialiseGrid();
        GenerateTerrain();
        Display();
    }


    #region Initialising

    /// <summary>
    /// Creates and sets up all of the tiles.
    /// </summary>
    private void InitialiseTiles()
    {
        allTiles = new List<Tile>();
        GenerateTiles();

        for (int i = 0; i < allTiles.Count; i++)
        {
            AddPossibleConnections(allTiles[i], allTiles);
        }
    }


    /// <summary>
    /// Creates 3 rotated versions for each original tile and adds them all to the allTiles list.
    /// </summary>
    private void GenerateTiles()
    {
        for (int i = 0; i < originalTiles.Length; i++)
        {
            List<Tile> rotatedTiles = CreateRotatedTiles(originalTiles[i]);

            for (int j = 0; j < rotatedTiles.Count; j++)
            {
                allTiles.Add(rotatedTiles[j]);
            }
        }
    }


    /// <summary>
    /// Creates and returns 4 instances of the given tile that face N/E/S/W
    /// </summary>
    private List<Tile> CreateRotatedTiles(Tile original)
    {
        List<Tile> rotatedTiles = new List<Tile>();
        rotatedTiles.Add(original);

        // for each rotated tile
        for (int i = 1; i < 4; i++)
        {
            Tile newTile = new Tile();
            newTile.rotationIndex = i;
            newTile.prefab = original.prefab;

            int socketsCount = newTile.sockets.Length;

            // rotates the sockets from the original tile to their new positions. Ie: original 0, 1, 2, 3, rotated 90deg 3, 0, 1, 2, 
            // rotated 180deg 2, 3, 0, 1...
            for (int j = 0; j < socketsCount; j++)
            {
                newTile.sockets[j] = original.sockets[(j + socketsCount - i) % socketsCount];
            }

            rotatedTiles.Add(newTile);
        }

        return rotatedTiles;
    }


    /// <summary>
    /// Sets the tiles that could connect to the given tile for each direction N/E/S/W
    /// </summary>
    /// <param name="tile">The tile being modified</param>
    /// <param name="availableTiles">The tile list that could connect to tile</param>
    private void AddPossibleConnections(Tile tile, List<Tile> availableTiles)
    {
        int socketsCount = tile.sockets.Length;

        // for each socket (N/E/S/W)
        for (int i = 0; i < socketsCount; i++)
        {
            for (int j = 0; j < availableTiles.Count; j++)
            {
                if (tile.sockets[i].FitsInto(availableTiles[j].sockets[(i + oppositeDirModifier) % socketsCount]))
                {
                    tile.possibleConnectors[i].Add(availableTiles[j]);
                }
            }
        }
    }


    /// <summary>
    /// Creates and populates the grid with cell tiles
    /// </summary>
    private void InitialiseGrid()
    {
        grid = new FIGrid(gridSize, allTiles);
    }

    #endregion

    #region Generate Terrain

    private void GenerateTerrain()
    {
        while (!grid.WaveFunctionCollapsed())
        {
            Iterate();
        }
    }


    /// <summary>
    /// Iterates through the grid, collapsing the cells with the lowest enttopy, then handles the changes that causes in neighbouring cells
    /// </summary>
    private void Iterate()
    {
        Vector2Int lowestEntropy = grid.GetLowestEntropy();
        grid.CollapsePossibleTiles(lowestEntropy);
        Propagate(lowestEntropy);
    }


    /// <summary>
    /// Applies a ripple effect to the rest of the grid based on the changes made at the given coords.
    /// </summary>
    /// <param name="coords">The coordinates of the cell that has had changes made</param>
    private void Propagate(Vector2Int coords)
    {
        Queue<Cell> q = new Queue<Cell>();
        q.Enqueue(grid.GetCell(coords));

        //while still cells to examine
        while (q.Count > 0)
        {
            // take the next cell
            Cell currentCell = q.Dequeue();
            Cell[] neighbours = grid.GetNeighbouringCells(currentCell);

            // for each neighbour/direction
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] != null)
                {
                    // possible connections and neighbours in the given direction
                    List<Connection> validConnections = currentCell.GetValidConnections(i);

                    if (!neighbours[i].Collapsed())
                    {
                        List<Tile> possibleNeighbourTiles = neighbours[i].possibleTiles;
                        bool addToStack = false;

                        for (int j = 0; j < possibleNeighbourTiles.Count; j++)
                        {
                            Connection[] possibleSockets = possibleNeighbourTiles[j].sockets;
                            int index = (i + oppositeDirModifier) % possibleSockets.Length;

                            if (!CheckSocketsMatch(possibleSockets[index], validConnections))
                            {
                                possibleNeighbourTiles.RemoveAt(j);
                                j--;
                                addToStack = true;
                            }
                        }

                        if (addToStack)
                        {
                            q.Enqueue(neighbours[i]);
                            neighbours[i].possibleTiles = possibleNeighbourTiles;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// returns true if connection a fits anything in list b
    /// </summary>
    private bool CheckSocketsMatch(Connection a, List<Connection> b)
    {
        for (int i = 0; i < b.Count; i++)
        {
            if (a == b[i])
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Display
    private void Display()
    {
        for (int row = 0; row < grid.grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.grid.GetLength(1); col++)
            {
                GameObject temp = Instantiate(grid.GetCell(col, row).possibleTiles[0].prefab, new Vector3(col, 0, row), Quaternion.identity, transform);
                temp.name = "" + col + ", " + row + ": " + grid.GetCell(col, row).possibleTiles[0].rotationIndex;
                switch (grid.GetCell(col, row).possibleTiles[0].rotationIndex)
                {
                    case 0:
                        temp.transform.eulerAngles = new Vector3(0, 0, 0);
                        break;
                    case 1:
                        temp.transform.eulerAngles = new Vector3(0, 90, 0);
                        break;
                    case 2:
                        temp.transform.eulerAngles = new Vector3(0, 180, 0);
                        break;
                    case 3:
                        temp.transform.eulerAngles = new Vector3(0, 270, 0);
                        break;
                    default:
                        break;
                }

            }
        }
    }

    #endregion
}
