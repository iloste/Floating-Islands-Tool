using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCWFC : MonoBehaviour
{
    public static MCWFC instance = null;
    [SerializeField] GameObject[] prefabs;

    [SerializeField] Vector3Int gridSize;
    [SerializeField] GameObject sg;

    [SerializeField] MCTile[] originalTiles;
    List<MCTile> allTiles;

    MCVertex[,,] vertices;
    MCCube[,,] cubes;

    MCGrid grid;

    int oppositeDirModifier = 2;
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

        //to do: check this should be *2
    }


    void Start()
    {
        InitialiseVertices();
        InitialiseTiles();
        InitialiseCells();
        InitialiseCubes();

     
        //vertices[1, 1, 1].full = true;
        //vertices[2, 1, 1].full = true;
        //UpdateCubes();
        //Iterate(new Vector3Int(1, 1, 1));
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


    private void InitialiseCells()
    {
        grid = new MCGrid(gridSize * 2, allTiles);
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
    }


    /// <summary>
    /// Creates the cells for the grid. Each cell has 8 vertices
    /// </summary>
    private void InitialiseCubes()
    {
        cubes = new MCCube[gridSize.x, gridSize.y, gridSize.z];

        for (int x = 0; x < cubes.GetLength(0); x++)
        {
            for (int y = 0; y < cubes.GetLength(1); y++)
            {
                for (int z = 0; z < cubes.GetLength(2); z++)
                {
                    cubes[x, y, z] = new MCCube();
                    cubes[x, y, z].coords = new Vector3Int(x, y, z);

                    #region Get Vertices
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

                    cubes[x, y, z].vertices = verts;
                    #endregion

                    #region Get Cells
                    MCCell[] cells = new MCCell[8];
                    // bottom vertices
                    cells[0] = grid.GetCell(x * 2, y * 2, z * 2);
                    cells[1] = grid.GetCell(x * 2, y * 2, z * 2 + 1);
                    cells[2] = grid.GetCell(x * 2 + 1, y * 2, z * 2 + 1);
                    cells[3] = grid.GetCell(x * 2 + 1, y * 2, z * 2);

                    // top verticies
                    cells[4] = grid.GetCell(x * 2, y * 2 + 1, z * 2);
                    cells[5] = grid.GetCell(x * 2, y * 2 + 1, z * 2 + 1);
                    cells[6] = grid.GetCell(x * 2 + 1, y * 2 + 1, z * 2 + 1);
                    cells[7] = grid.GetCell(x * 2 + 1, y * 2 + 1, z * 2);

                    cubes[x, y, z].cells = cells;
                    #endregion
                }
            }
        }
    }

    #endregion


    #region WaveCollapse


    /// <summary>
    /// Iterates through the grid, collapsing the cells with the lowest enttopy, then handles the changes that causes in neighbouring cells
    /// </summary>
    public void Iterate(Vector3Int gridCoords)
    {
        grid.ResetPossibilitySpace();
        // converts from vertex position to cell position because there are twice as many cells as there are vertices.
        gridCoords = gridCoords * 2 - new Vector3Int(1, 1, 1);
        grid.GetCell(gridCoords).CollapsePossibleTiles();
        Propagate(gridCoords);
        DisplayCubes();
    }


    /// <summary>
    /// Applies a ripple effect to the rest of the grid based on the changes made at the given coords.
    /// </summary>
    /// <param name="coords">The coordinates of the cell that has had changes made</param>
    private void Propagate(Vector3Int coords)
    {
        Queue<MCCell> q = new Queue<MCCell>();
        q.Enqueue(grid.GetCell(coords));

        //while still cells to examine
        while (q.Count > 0)
        {
            // take the next cell
            MCCell currentCell = q.Dequeue();
            MCCell[] neighbours = grid.GetNeighbouringCells(currentCell);

            // for each neighbour/direction N\E\S\W\U\D
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] != null)
                {
                    // possible connections in the given direction
                    List<Connection> validConnections = currentCell.GetValidConnections(i);

                    if (!neighbours[i].Collapsed())
                    {
                        List<MCTile> possibleNeighbourTiles = neighbours[i].possibleTiles;
                        bool addToStack = false;

                        for (int j = 0; j < possibleNeighbourTiles.Count; j++)
                        {
                            Connection[] possibleSockets = possibleNeighbourTiles[j].sockets;
                            int index = (GetOppositeDirection(i)) % possibleSockets.Length;
                            // int index = (i + oppositeDirModifier) % possibleSockets.Length;

                            // if possible tile doesn't match, remove it
                            if (!CheckSocketsMatch(possibleSockets[index], validConnections))
                            {
                                neighbours[i].RemovePossibleTile(j);
                                j--;
                                addToStack = true;//true if something has changed
                            }
                        }

                        if (addToStack)// to do: add to stack if not collapsed and maybe only if not already in the stack
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






    /// <summary>
    /// Sets the desired vertex to true and updates the cells.
    /// </summary>
    /// <param name="coords"></param>
    public void FillVertex(Vector3Int coords)
    {
        vertices[coords.x, coords.y, coords.z].full = true;
        UpdateCubes();
        if (grid.grid[2, 2, 2].Collapsed())
        {

        }
        Iterate(coords);
    }


    /// <summary>
    /// Sets the desired vertex to false and updates the cells.
    /// </summary>
    /// <param name="coords"></param>
    public void ClearVertex(Vector3Int coords)
    {
        vertices[coords.x, coords.y, coords.z].full = false;
        UpdateCubes();
    }


    /// <summary>
    /// Goes through each cell and calculates which tiles it will show.
    /// </summary>
    private void UpdateCubes()
    {
        for (int x = 0; x < cubes.GetLength(0); x++)
        {
            for (int y = 0; y < cubes.GetLength(1); y++)
            {
                for (int z = 0; z < cubes.GetLength(2); z++)
                {
                    cubes[x, y, z].Update(prefabs);
                }
            }
        }

    }


    private void DisplayCubes()
    {
        for (int x = 0; x < cubes.GetLength(0); x++)
        {
            for (int y = 0; y < cubes.GetLength(1); y++)
            {
                for (int z = 0; z < cubes.GetLength(2); z++)
                {
                    cubes[x, y, z].Update2();
                }
            }
        }
    }
}