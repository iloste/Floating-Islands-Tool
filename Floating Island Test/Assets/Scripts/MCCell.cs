using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCell
{

    public enum MCValidConnection
    {
        Nothing,
        Something,
        Middle,
    }

    public bool[] validConnections { get; private set; }
    public MCValidConnection[] validConnections2 { get; private set; }
    public Connection[] connections;

    private List<MCTile> allTiles;
    public List<MCTile> possibleTiles { get; private set; }
    public Vector3Int coords;
    public Vector3 worldPosition;
    public GameObject debugGO;
    public bool visited = false;

    public bool TileExists { get; private set; }


    public MCCell(List<MCTile> allTiles, Vector3Int coords)
    {
        Initialise(allTiles, coords);
    }


    private void Initialise(List<MCTile> allTiles, Vector3Int coords)
    {
        this.allTiles = allTiles;
        possibleTiles = new List<MCTile>();
        //  impossibleTiles = new List<MCTile>();

        this.coords = coords;
        worldPosition = new Vector3(coords.x / 2f, coords.y / 2f, coords.z / 2f);

        validConnections = new bool[6];
        validConnections2 = new MCValidConnection[6];
        connections = new Connection[6];
    }


    public void SetValidConnections(bool[] validConnections)
    {
        this.validConnections = validConnections;
    }

    public void SetValidConnections(MCValidConnection[] validConnections)
    {
        this.validConnections2 = validConnections;
    }


    /// <summary>
    /// Returns true if there is only one possible tile left
    /// </summary>
    /// <returns></returns>
    public bool Collapsed()
    {
        if (possibleTiles.Count > 1)
        {
            if (possibleTiles.Count == 0)
            {
                Debug.LogError("Cell " + coords + " has no possible tiles. Valid Connections: " + validConnections2[0] + " " + validConnections2[1] + " " + validConnections2[2] + " " + validConnections2[3] + " " + validConnections2[4] + " " + validConnections2[5]);
            }
            return false;
        }

        return true;
    }


    public void SetTileExists(bool exists)
    {
        if (exists)
        {
            debugGO.transform.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            debugGO.transform.GetComponent<Renderer>().material.color = Color.red;
        }

        if (MCWFC.instance.displayActiveCellLocations)
        {
            debugGO.SetActive(exists);
        }
        else
        {
            debugGO.SetActive(false);
        }

        TileExists = exists;
    }


    public void RemovePossibleTile(int index)
    {
        if (index < possibleTiles.Count)
        {
            possibleTiles.RemoveAt(index);
        }

        for (int i = 0; i < validConnections.Length; i++)
        {
            if (validConnections[i] == true)
            {
                if (possibleTiles.Count == 0)
                {
                    Debug.LogError("Cell " + coords + " has no possible tiles. Valid Connections: " + validConnections2[0] +" "+ validConnections2[1] +" "+ validConnections2[2] +" "+ validConnections2[3] +" "+ validConnections2[4] +" "+ validConnections2[5]);
                }
                break;
            }
        }


    }


    /// <summary>
    /// Picks one of it's possible tiles to be the set tile.
    /// </summary>
    public void WaveFunctionCollapse(MCCell[] neighbours)
    {

        // removes tiles that won't fit based on neighbour possible tiles
        for (int direction = 0; direction < neighbours.Length; direction++)
        {
            if (neighbours[direction] != null)
            {
                // remove neighbours possible tiles based on current cells tiles
                int oppositeDirection = GetOppositeDirection(direction);

                List<Connection> neighboursValidConnections = neighbours[direction].GetValidConnections(oppositeDirection);

                // for each nighbour tile
                // if it's connection isn't in valid connections, remove it.
                for (int i = 0; i < possibleTiles.Count; i++)
                {
                    Connection nextConnection = possibleTiles[i].sockets[direction];

                    if (!CheckSocketsMatch(nextConnection, neighboursValidConnections))
                    {
                        // possibleTiles.RemoveAt(i);
                        // i--;
                        RemovePossibleTile(i);
                        i--;
                    }
                }
            }
        }

        if (possibleTiles.Count > 0)
        {
            // picks a tile at random
            int random = Random.Range(0, possibleTiles.Count);
            MCTile chosen = possibleTiles[random];
            RemovePossibleTile(random);
            //possibleTiles.RemoveAt(random);

            // stores the chosen tile in possible tiles
            possibleTiles = new List<MCTile>();
            possibleTiles.Add(chosen);
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


    /// <summary>
    /// removes any possible tile that doesn't match the valid N/E/S/W/U/D connections that the cell has. 
    /// (I.e. if the cell connects north and east, the possible tiles have to connect north and east.
    /// </summary>
    private void RemoveTilesThatDontFit()
    {
        if (coords == new Vector3Int(3, 2, 3))
        {

        }
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            //if (!TileFitsValidConnections(possibleTiles[i]))
            if (!TileFitsValidConnections2(possibleTiles[i]))
            {
                // impossibleTiles.Add(possibleTiles[i]);
                //possibleTiles.RemoveAt(i);
                RemovePossibleTile(i);
                i--;
            }
        }
    }


    /// <summary>
    /// Returns true if the given tile has connections in the same directions as the cell has.
    /// </summary>
    /// <param name="possibleTile"></param>
    /// <returns></returns>
    private bool TileFitsValidConnections(MCTile possibleTile)
    {
        Connection[] connectors = possibleTile.sockets;

        for (int i = 0; i < connections.Length; i++)
        {
            if (connectors[i].number == -1 && validConnections[i] ||
                connectors[i].number != -1 && !validConnections[i])
            {
                return false;
            }
        }

        return true;
    }
    private bool TileFitsValidConnections2(MCTile possibleTile)
    {
        Connection[] connectors = possibleTile.sockets;

        for (int i = 0; i < connections.Length; i++)
        {
            if ((connectors[i].number == 99 && validConnections2[i] != MCValidConnection.Middle))
            {
                return false;
            }
            else if ((connectors[i].number == -1 && validConnections2[i] != MCValidConnection.Nothing))
            {
                return false;
            }
            else if ((connectors[i].number != -1 && connectors[i].number != 99) && validConnections2[i] != MCValidConnection.Something)
            {
                return false;
            }
        }

        return true;
    }


    public void ResetPossiblitySpace()
    {
        possibleTiles = new List<MCTile>(allTiles);
        if (coords == new Vector3Int(2, 2, 2))
        {

        }
        RemoveTilesThatDontFit();
    }


    /// <summary>
    /// Returns a list of valid connections for the given direction
    /// </summary>
    public List<Connection> GetValidConnections(int dir)
    {
        List<Connection> validConnections = new List<Connection>();

        for (int i = 0; i < possibleTiles.Count; i++)
        {
            // possible connectors in the given direction
            List<MCTile> possibleConnectors = possibleTiles[i].possibleConnectors[dir];

            for (int j = 0; j < possibleConnectors.Count; j++)
            {
                bool addToList = true;
                Connection[] possibleSockets = possibleConnectors[j].sockets;

                for (int x = 0; x < validConnections.Count; x++)
                {
                    if (validConnections.Contains(possibleSockets[GetOppositeDirection(dir)]))
                    // if (validConnections.Contains(possibleSockets[(dir + 2) % possibleSockets.Length]))
                    {
                        addToList = false;
                        break;
                    }
                }

                if (addToList)
                {
                    validConnections.Add(possibleConnectors[j].sockets[GetOppositeDirection(dir)]);
                    //  validConnections.Add(possibleConnectors[j].sockets[(dir + 2) % 4]);
                }
            }
        }

        return validConnections;
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
}
