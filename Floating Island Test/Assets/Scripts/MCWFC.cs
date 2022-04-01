using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCWFC : MonoBehaviour
{
    public static MCWFC instance = null;
    [SerializeField] GameObject[] prefabs;

    [SerializeField] Vector3Int gridSize;

    [SerializeField] MCTile[] originalTiles;
    List<MCTile> allTiles;

    MCInterfaceGrid grid;

    [SerializeField] GameObject vertexDebugPrefab;
    [SerializeField] bool displayVertices;
    [SerializeField] GameObject cellDebugPrefab;
    [SerializeField] public bool displayActiveCellLocations;
    [SerializeField] GameObject cubeDebugPrefab;
    [SerializeField] bool displayCubeLocations;


    const int MAX_ROTATIONS = 4;


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
        InitialiseTiles();

        grid = new MCInterfaceGrid(gridSize, allTiles, vertexDebugPrefab, cellDebugPrefab, cubeDebugPrefab);

        DisplayVertices();

        if (displayCubeLocations)
        {
            DisplayCubeLocations();
        }


        #region Debugs
        //for (int i = 0; i < allTiles.Count; i++)
        //{
        //    GameObject temp = GameObject.Instantiate(allTiles[i].prefab, new Vector3(i * 2, 0, -5), Quaternion.identity);
        //    temp.transform.eulerAngles = new Vector3(0, 90 * allTiles[i].rotationIndex, 0);

        //}

        //vertices[1, 1, 1].full = true;
        //vertices[2, 1, 1].full = true;
        //vertices[2, 1, 2].full = true;
        //UpdateCubesCells();
        //Iterate(new Vector3Int(1, 1, 1)); 
        #endregion
    }


    #region Initialising

    /// <summary>
    /// Creates and sets up all of the tiles.
    /// </summary>
    private void InitialiseTiles()
    {
        allTiles = new List<MCTile>();
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
            List<MCTile> rotatedTiles = CreateRotatedTiles(originalTiles[i]);

            for (int j = 0; j < rotatedTiles.Count; j++)
            {
                allTiles.Add(rotatedTiles[j]);
            }
        }
    }


    /// <summary>
    /// Creates and returns 4 instances of the given tile that face N/E/S/W
    /// </summary>
    private List<MCTile> CreateRotatedTiles(MCTile original)
    {
        List<MCTile> rotatedTiles = new List<MCTile>();
        rotatedTiles.Add(original);

        // for each rotated tile
        for (int i = 1; i < MAX_ROTATIONS; i++)
        {
            MCTile newTile = new MCTile();
            newTile.rotationIndex = i;
            newTile.prefab = original.prefab;
            newTile.name = original.name + " Rotation Index: " + i;
            int socketsCount = newTile.sockets.Length;

            // rotates the sockets from the original tile to their new positions. Ie: original 0, 1, 2, 3, rotated 90deg 3, 0, 1, 2, 
            // rotated 180deg 2, 3, 0, 1...
            for (int j = 0; j < MAX_ROTATIONS; j++)
            {
                newTile.sockets[j] = original.sockets[(j + MAX_ROTATIONS - i) % MAX_ROTATIONS];
            }

            newTile.sockets[4] = original.sockets[4];
            newTile.sockets[5] = original.sockets[5];

            #region Rotate Up/Down connections
            if (newTile.sockets[4].number != -1)
            {
                switch (i)
                {
                    case 1:
                        newTile.sockets[4].spConnection = MCTile.SpecialConnection.VerticalRotated1;
                        break;
                    case 2:
                        newTile.sockets[4].spConnection = MCTile.SpecialConnection.VerticalRotated2;
                        break;
                    case 3:
                        newTile.sockets[4].spConnection = MCTile.SpecialConnection.VerticalRotated3;
                        break;
                    default:
                        break;
                }
            }

            if (newTile.sockets[5].number != -1)
            {
                switch (i)
                {
                    case 1:
                        newTile.sockets[5].spConnection = MCTile.SpecialConnection.VerticalRotated1;
                        break;
                    case 2:
                        newTile.sockets[5].spConnection = MCTile.SpecialConnection.VerticalRotated2;
                        break;
                    case 3:
                        newTile.sockets[5].spConnection = MCTile.SpecialConnection.VerticalRotated3;
                        break;
                    default:
                        break;
                }
            }


            #endregion
            rotatedTiles.Add(newTile);
        }

        return rotatedTiles;
    }


    /// <summary>
    /// Sets the tiles that could connect to the given tile for each direction N/E/S/W
    /// </summary>
    /// <param name="tile">The tile being modified</param>
    /// <param name="availableTiles">The tile list that could connect to tile</param>
    private void AddPossibleConnections(MCTile tile, List<MCTile> availableTiles)
    {
        int socketsCount = tile.sockets.Length;

        // for each socket (N/E/S/W/U/D)
        for (int i = 0; i < socketsCount; i++)
        {
            for (int j = 0; j < availableTiles.Count; j++)
            {
                //  if (tile.sockets[i].FitsInto(availableTiles[j].sockets[(i + oppositeDirModifier) % socketsCount]))
                if (tile.sockets[i].FitsInto(availableTiles[j].sockets[GetOppositeDirection(i)]))
                {
                    tile.possibleConnectors[i].Add(availableTiles[j]);
                }
            }
        }
    }

    #endregion



    /// <summary>
    /// returns the int that represents the opposite direction in the arrays used to store tiles
    /// </summary>
    /// <param name="currentDirection"></param>
    /// <returns></returns>
    private int GetOppositeDirection(int currentDirection)
    {
        switch (currentDirection)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 0;
            case 3:
                return 1;
            case 4:
                return 5;
            case 5:
                return 4;
            default:
                Debug.LogError("Direction does not exist!");
                return -1;
        }
    }


    #region WaveCollapse


    /// <summary>
    /// Iterates through the grid, collapsing the cells with the lowest enttopy, then handles the changes that causes in neighbouring cells
    /// </summary>
    public void Iterate(Vector3Int coords)
    {
        // to do: I think any coorinate convertion should happen in the interface grid class
        // converts from vertex position to cell position because there are twice as many cells as there are vertices.
        coords = coords * 2 - new Vector3Int(1, 1, 1);

        grid.CollapsePossibleTiles(coords);
        Propagate(coords);

        while (!grid.GridCollapsed())
        {
            coords = grid.FindUncollapsedCell().coords;
            grid.CollapsePossibleTiles(coords);
            Propagate(coords);
        }

        DisplayCubes();
    }

    private void Propagate(Vector3Int coords)
    {
        Queue<MCCell> q = new Queue<MCCell>();
        q.Enqueue(grid.GetCell(coords));

        while (q.Count > 0)
        {
            // get current cell
            MCCell currentCell = q.Dequeue();
            // get current cell neighbours
            if (currentCell != null)
            {
                MCCell[] neighbours = grid.GetNeighbouringCells(currentCell);

                //for each neighbour

                for (int direction = 0; direction < neighbours.Length; direction++)
                {
                    if (neighbours[direction] != null)
                    {
                        bool changesMade = false;

                        if (!neighbours[direction].Collapsed())
                        {
                            if (neighbours[direction].coords == new Vector3Int(2, 2, 2))
                            {

                            }
                            // remove neighbours possible tiles based on current cells tiles
                            List<Connection> validConnections = currentCell.GetValidConnections(direction);
                            List<MCTile> possibleNeighbourTiles = neighbours[direction].possibleTiles;
                            int oppositeDirection = GetOppositeDirection(direction);

                            // for each nighbour tile
                            // if it's connection isn't in valid connections, remove it.
                            for (int i = 0; i < possibleNeighbourTiles.Count; i++)
                            {
                                Connection nextConnection = possibleNeighbourTiles[i].sockets[oppositeDirection];

                                if (!CheckSocketsMatch(nextConnection, validConnections))
                                {
                                    possibleNeighbourTiles.RemoveAt(i);
                                    i--;
                                    changesMade = true;
                                }
                            }
                        }

                        if (changesMade)
                        {
                            q.Enqueue(neighbours[direction]);
                        }
                    }
                }
            }
        }
    }


    


   


   

   


    #endregion




    #region Utility Functions

    /// <summary>
    /// Sets the desired vertex to true and updates the cells.
    /// </summary>
    /// <param name="coords"></param>
    public void FillVertex(Vector3Int coords)
    {
        // debug purposes
        // to do: remove this when the grid doesn't start at 0 on the y
       // coords += Vector3Int.up;

        grid.FillVertex(coords);
        Iterate(coords);
    }


    /// <summary>
    /// Sets the desired vertex to false and updates the cells.
    /// </summary>
    /// <param name="coords"></param>
    public void ClearVertex(Vector3Int coords)
    {
        // debug purposes
        // to do: remove this when the grid doesn't start at 0 on the y
       // coords += Vector3Int.up;
        grid.ClearVertex(coords);

        Iterate(coords);

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

    private void DisplayCubes()
    {
        grid.DisplayCubes();
    }


    private void DisplayVertices()
    {
        grid.DisplayVertices(displayVertices);
    }


    private void DisplayCubeLocations()
    {
        grid.DisplayCubes();
    }

    #endregion

}