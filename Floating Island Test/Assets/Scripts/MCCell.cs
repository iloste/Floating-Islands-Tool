using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCell
{
    public bool[] filledSockets { get; set; }
    public Connection[] connections;
    public List<MCTile> possibleTiles;
    public List<MCTile> impossibleTiles;
    public Vector3Int coords;
    public Vector3 worldPosition;
    public GameObject debugGO;
    public bool TileExists { get; private set; }


    public MCCell(List<MCTile> allTiles, Vector3Int coords)
    {
        this.possibleTiles = allTiles;
        impossibleTiles = new List<MCTile>();
        this.coords = coords;
        worldPosition = new Vector3(coords.x / 2f, coords.y / 2f, coords.z / 2f);
        filledSockets = new bool[6];
        connections = new Connection[6];
    }

    public bool Collapsed()
    {
        if (possibleTiles.Count > 1)
        {
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
            impossibleTiles.Add(possibleTiles[index]);
            possibleTiles.RemoveAt(index);
        }
    }

    /// <summary>
    /// Picks one of it's possible tiles to be the set tile.
    /// </summary>
    public void CollapsePossibleTiles()
    {
        // removes any possible tile that doesn't match the N/E/S/W/U/D connections that the cell has. 
        //(I.e. if the cell connects north and east, the possible tiles have to connect north and east.
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            if (!CheckPossibleTileFitsWithAllotedSockets(possibleTiles[i]))
            {
                impossibleTiles.Add(possibleTiles[i]);
                possibleTiles.RemoveAt(i);
                i--;
            }
        }

        if (possibleTiles.Count > 0)
        {
            // picks a tile at random
            int random = Random.Range(0, possibleTiles.Count);
            MCTile chosen = possibleTiles[random];
            possibleTiles.RemoveAt(random);

            // stores the rest in impossibleTiles
            for (int i = 0; i < possibleTiles.Count; i++)
            {
                impossibleTiles.Add(possibleTiles[i]);
            }

            // stores the chosen tile in possible tiles
            possibleTiles = new List<MCTile>();
            possibleTiles.Add(chosen); 
        }
    }


    /// <summary>
    /// Returns true if the given tile has non-null sockets in the same place directions as the cell.
    /// </summary>
    /// <param name="possibleTile"></param>
    /// <returns></returns>
    private bool CheckPossibleTileFitsWithAllotedSockets(MCTile possibleTile)
    {
        // to do: change the name to something more sensible
        Connection[] connectors = possibleTile.sockets;

        for (int i = 0; i < connections.Length; i++)
        {
            if (connectors[i].number == -1 && filledSockets[i] ||
                connectors[i].number != -1 && !filledSockets[i])
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Resets possible tiles to all tiles
    /// </summary>
    public void ResetPossibleTiles(List<MCTile> tiles)
    {
        impossibleTiles = new List<MCTile>();
        possibleTiles = new List<MCTile>(tiles);


        for (int i = 0; i < possibleTiles.Count; i++)
        {
            if (!CheckPossibleTileFitsWithAllotedSockets(possibleTiles[i]))
            {
                impossibleTiles.Add(possibleTiles[i]);
                possibleTiles.RemoveAt(i);
                i--;
            }
        }

        //List<MCTile> temp = new List<MCTile>();

        //for (int i = 0; i < possibleTiles.Count; i++)
        //{
        //    temp.Add(possibleTiles[i]);
        //}
        //for (int i = 0; i < impossibleTiles.Count; i++)
        //{
        //    temp.Add(impossibleTiles[i]);
        //}

        //possibleTiles = temp;
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
