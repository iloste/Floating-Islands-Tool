using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapse2D : MonoBehaviour
{
    [SerializeField] Tile[] originalTiles;
    [SerializeField] Vector2Int gridSize;
    List<Tile> allTiles;

    List<List<Cell>> grid;
    [SerializeField] bool debug;
    [SerializeField] bool randomSeed;

    int oppositeDirModifier = 2;

    private void Awake()
    {
        if (!randomSeed)
        {
            Random.InitState(0);
        }
        else
        {
            Random.InitState(Random.Range(0, 300000));

        }

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
        grid = new List<List<Cell>>();

        for (int col = 0; col < gridSize.x; col++)
        {
            grid.Add(new List<Cell>());

            for (int row = 0; row < gridSize.y; row++)
            {
                grid[col].Add(new Cell(new List<Tile>(allTiles), new Vector2(col, row)));
            }
        }
    }

    #endregion

    #region Generate Terrain

    private void GenerateTerrain()
    {
        while (!WaveFunctionCollapsed())
        {
            Iterate();
        }
    }


    /// <summary>
    /// returns true if all cells in the grid have only 1 possible tile. Else returns false.
    /// </summary>
    /// <returns></returns>
    private bool WaveFunctionCollapsed()
    {
        for (int row = 0; row < grid.Count; row++)
        {
            for (int col = 0; col < grid[row].Count; col++)
            {
                if (grid[col][row].possibleTiles.Count > 1)
                {
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Iterates through the grid, collapsing the cells with the lowest enttopy, then handles the changes that causes in neighbouring cells
    /// </summary>
    private void Iterate()
    {
        Vector2Int lowestEntropy = GetLowestEntropy();
        CollapsePossibleTiles(grid[lowestEntropy.x][lowestEntropy.y]);
        Propagate(lowestEntropy);
    }


    /// <summary>
    /// Returns the coordinates of the cell with the fewest possible tiles above 1.
    /// </summary>
    private Vector2Int GetLowestEntropy()
    {
        Vector2Int lowestIndex = Vector2Int.zero;

        for (int row = 0; row < grid.Count; row++)
        {
            for (int col = 0; col < grid[row].Count; col++)
            {
                if (grid[col][row].possibleTiles.Count > 1)
                {
                    // todo return coords if the value is two else do this? That might be quicker because 2 is the lowest value needed for returning.
                    if (grid[col][row].possibleTiles.Count < grid[lowestIndex.x][lowestIndex.y].possibleTiles.Count || grid[lowestIndex.x][lowestIndex.y].possibleTiles.Count <= 1)
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
    /// Picks one of it's possible tiles to be the set tile.
    /// </summary>
    private void CollapsePossibleTiles(Cell cell)
    {
        int random = Random.Range(0, cell.possibleTiles.Count);

        // To do: Remove debug when not needed
        if (debug)
        {
            random = 0;
            debug = false;
        }

        Tile chosen = cell.possibleTiles[random];
        cell.possibleTiles = new List<Tile>();
        cell.possibleTiles.Add(chosen);
    }


    /// <summary>
    /// Applies a ripple effect to the rest of the grid based on the changes made at the given coords.
    /// </summary>
    /// <param name="coords">The coordinates of the cell that has had changes made</param>
    private void Propagate(Vector2Int coords)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(coords);

        //while still cells to examine
        while (q.Count > 0)
        {
            // take the next cell
            Vector2Int currentCoords = q.Dequeue();
            Cell currentCell = grid[currentCoords.x][currentCoords.y];
            Vector2Int[] neighboursCoords = GetNeighbourCoords(currentCoords);
            Cell[] neighbours = GetCells(neighboursCoords);

            // for each neighbour/direction
            for (int i = 0; i < neighboursCoords.Length; i++)
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
                            q.Enqueue(neighboursCoords[i]);
                            grid[neighboursCoords[i].x][neighboursCoords[i].y].possibleTiles = possibleNeighbourTiles;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// returns the coordinates for the N/E/S/W neighbours.
    /// If the neighbour is invalid, returns Vector2(-1, -1)
    /// </summary>
    private Vector2Int[] GetNeighbourCoords(Vector2Int coords)
    {
        Vector2Int[] Neighbours = new Vector2Int[4];

        #region Get the coodinates

        if (coords.y + 1 < grid.Count)
        {
            Neighbours[0] = new Vector2Int(coords.x, coords.y + 1);
        }
        else
        {
            Neighbours[0] = new Vector2Int(-1, -1);
        }

        if (coords.x + 1 < grid.Count)
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

    private Cell[] GetCells(Vector2Int[] coords)
    {
        Cell[] cells = new Cell[coords.Length];

        for (int i = 0; i < coords.Length; i++)
        {
            if (coords[i].x >= 0)
            {
                cells[i] = grid[coords[i].x][coords[i].y];
            }
        }

        return cells;
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


    private bool CheckSocketsFit(Connection a, Connection b)
    {
        return a.FitsInto(b);

    }


    /// <summary>
    /// Returns a list of all valid connections that fit into the sockest in the given direction of the list of tiles.
    /// If a valid connection appers twice it is only added once to the returned list.
    /// </summary>
    /// <param name="tiles">Tiles that are being checked</param>
    /// <param name="dir">Direction that is being checked N/E/S/W</param>
    /// <returns></returns>
    private List<Connection> GetValidConnections(List<Tile> tiles, int dir)
    {
        List<Connection> validConnections = new List<Connection>();

        for (int i = 0; i < tiles.Count; i++)
        {
            // possible connectors in the given direction
            List<Tile> possibleConnectors = tiles[i].possibleConnectors[dir];

            for (int j = 0; j < possibleConnectors.Count; j++)
            {
                bool addToList = true;
                Connection[] possibleSockets = possibleConnectors[j].sockets;

                for (int x = 0; x < validConnections.Count; x++)
                {
                    if (validConnections.Contains(possibleSockets[(dir + oppositeDirModifier) % possibleSockets.Length]))
                    {
                        addToList = false;
                        break;
                    }
                }

                if (addToList)
                {
                    validConnections.Add(possibleConnectors[j].sockets[(dir + 2) % 4]);
                }
            }
        }

        return validConnections;
    }








    #endregion

    #region Display
    private void Display()
    {
        for (int row = 0; row < grid.Count; row++)
        {
            for (int col = 0; col < grid[row].Count; col++)
            {
                GameObject temp = Instantiate(grid[col][row].possibleTiles[0].prefab, new Vector3(col, 0, row), Quaternion.identity, transform);
                temp.name = "" + col + ", " + row + ": " + grid[col][row].possibleTiles[0].rotationIndex;
                switch (grid[col][row].possibleTiles[0].rotationIndex)
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

    #region Not Used

    private Tile CreateRotatedTile(Tile tile, int rotationIndex)
    {
        Tile newTile = new Tile();
        newTile.rotationIndex = rotationIndex;
        newTile.prefab = tile.prefab;

        switch (rotationIndex)
        {
            case 1:
                newTile.sockets[0] = tile.sockets[3];
                newTile.sockets[1] = tile.sockets[0];
                newTile.sockets[2] = tile.sockets[1];
                newTile.sockets[3] = tile.sockets[2];
                break;
            case 2:
                newTile.sockets[0] = tile.sockets[2];
                newTile.sockets[1] = tile.sockets[3];
                newTile.sockets[2] = tile.sockets[0];
                newTile.sockets[3] = tile.sockets[1];
                break;
            case 3:
                newTile.sockets[0] = tile.sockets[1];
                newTile.sockets[1] = tile.sockets[2];
                newTile.sockets[2] = tile.sockets[3];
                newTile.sockets[3] = tile.sockets[0];
                break;
            default:
                Debug.LogError("Invalid rotation Index");
                break;
        }

        return newTile;
    }


    //private bool CheckSockets(int a, int b, Tile.SpecialConnection c, Tile.SpecialConnection d)
    //{
    //    if (a == b)
    //    {
    //        if (c == Tile.SpecialConnection.Symmetrical && d == Tile.SpecialConnection.Symmetrical)
    //        {
    //            return true;
    //        }
    //        else if (c == Tile.SpecialConnection.Flipped && d == Tile.SpecialConnection.None ||
    //            c == Tile.SpecialConnection.None && d == Tile.SpecialConnection.Flipped)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}



    ///// <summary>
    ///// returns true if connection a fits anything in list b
    ///// </summary>
    //private bool CheckSockets(Connection a, Connection[] b)
    //{
    //    for (int i = 0; i < b.Length; i++)
    //    {
    //        if (a.number == b[i].number)
    //        {
    //            if (a.spConnection == Tile.SpecialConnection.Symmetrical && b[i].spConnection == Tile.SpecialConnection.Symmetrical)
    //            {
    //                return true;
    //            }
    //            else if (a.spConnection == Tile.SpecialConnection.Flipped && b[i].spConnection == Tile.SpecialConnection.None ||
    //                a.spConnection == Tile.SpecialConnection.None && b[i].spConnection == Tile.SpecialConnection.Flipped)
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}




    //private List<int> GetValidConnections(List<Tile> possibleTiles, int d)
    //{
    //    List<int> validConnections = new List<int>();
    //    List<Tile.SpecialConnection> spConnection = new List<Tile.SpecialConnection>();

    //    for (int i = 0; i < possibleTiles.Count; i++)
    //    {
    //        for (int j = 0; j < possibleTiles[i].possibleConnectors[d].Count; j++)
    //        {
    //            if (!validConnections.Contains(possibleTiles[i].possibleConnectors[d][j].sockets[(d + 2) % 4]))
    //            {
    //                validConnections.Add(possibleTiles[i].possibleConnectors[d][j].sockets[(d + 2) % 4]);
    //            }
    //        }
    //    }

    //    return validConnections;
    //}

    //private List<Tile.SpecialConnection> GetValidSpecialConnections(List<Tile> possibleTiles, int d)
    //{
    //    List<Tile.SpecialConnection> validConnections = new List<Tile.SpecialConnection>();

    //    for (int i = 0; i < possibleTiles.Count; i++)
    //    {
    //        for (int j = 0; j < possibleTiles[i].possibleConnectors[d].Count; j++)
    //        {

    //            if (possibleTiles[i].possibleConnectors[d][j].specialConnections[(d + 2) % 4] == Tile.SpecialConnection.Symmetrical)
    //            {
    //                if (!validConnections.Contains(possibleTiles[i].possibleConnectors[d][j].specialConnections[(d + 2) % 4]))
    //                {
    //                    validConnections.Add(possibleTiles[i].possibleConnectors[d][j].specialConnections[(d + 2) % 4]);
    //                }
    //            }
    //            else if (possibleTiles[i].possibleConnectors[d][j].specialConnections[(d + 2) % 4] == Tile.SpecialConnection.Flipped)
    //            {
    //                if (!validConnections.Contains(Tile.SpecialConnection.None))
    //                {
    //                    validConnections.Add(Tile.SpecialConnection.None);
    //                }
    //            }
    //            else if (possibleTiles[i].possibleConnectors[d][j].specialConnections[(d + 2) % 4] == Tile.SpecialConnection.None)
    //            {
    //                if (!validConnections.Contains(Tile.SpecialConnection.Flipped))
    //                {
    //                    validConnections.Add(Tile.SpecialConnection.Flipped);
    //                }
    //            }


    //        }
    //    }

    //    return validConnections;
    //}


    ///// <summary>
    ///// List2 is modified and returned if needed
    ///// </summary>
    ///// <param name="list1"></param>
    ///// <param name="list2"></param>
    ///// <returns></returns>
    //private List<Tile> CompareConnections(List<Tile> list1, int direction, List<Tile> list2)
    //{
    //    bool match = false;

    //    for (int i = 0; i < list2.Count; i++)
    //    {
    //        for (int j = 0; j < list1.Count; j++)
    //        {
    //            if (list2[i].sockets[(i + 2) % 4] == list1[j].sockets[i] &&
    //                list2[i].specialConnections[(i + 2) % 4] == list1[j].specialConnections[i])
    //            {
    //                match = true;
    //                break;
    //            }
    //        }

    //        if (!match)
    //        {
    //            list2.RemoveAt(i);
    //            i--;
    //        }
    //    }

    //    return list2;
    //}
    #endregion
}
