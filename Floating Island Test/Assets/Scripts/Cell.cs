using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell 
{
    public List<Tile> possibleTiles;
    public List<Tile> impossibleTiles;
    public Vector2Int gridCoords;
    public int testNumber = 0;
    public bool displayed;
    public GameObject GO;

    public Cell()
    {
        possibleTiles = new List<Tile>();
        impossibleTiles = new List<Tile>();
        gridCoords = Vector2Int.zero;
    }

    public Cell(List<Tile> possibleTiles, Vector2Int gridCoords)
    {
        this.possibleTiles = possibleTiles;
        impossibleTiles = new List<Tile>();
        this.gridCoords = gridCoords;
    }


    public bool Collapsed()
    {
        if (possibleTiles.Count > 1)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Picks one of it's possible tiles to be the set tile.
    /// </summary>
    public void CollapsePossibleTiles()
    {
        int random = Random.Range(0, possibleTiles.Count);
        Tile chosen = possibleTiles[random];
        possibleTiles.RemoveAt(random);
        impossibleTiles = new List<Tile>(possibleTiles);
        possibleTiles = new List<Tile>();
        possibleTiles.Add(chosen);
    }


    public void ResetPossibleTiles()
    {
        Tile chosen = possibleTiles[0];
        possibleTiles = new List<Tile>(impossibleTiles);
        impossibleTiles = new List<Tile>();
        possibleTiles.Add(chosen);
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
            List<Tile> possibleConnectors = possibleTiles[i].possibleConnectors[dir];

            for (int j = 0; j < possibleConnectors.Count; j++)
            {
                bool addToList = true;
                Connection[] possibleSockets = possibleConnectors[j].sockets;

                for (int x = 0; x < validConnections.Count; x++)
                {
                    if (validConnections.Contains(possibleSockets[(dir + 2) % possibleSockets.Length]))
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


    public void RemoveDisplayTile()
    {
        GameObject.Destroy(GO);
        displayed = false;
        //GO = null;
    }
}
