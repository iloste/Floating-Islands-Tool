using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell 
{
    List<Tile> possibleTiles;
    List<Tile> impossibleTiles;
    Vector2 gridCoords;

    public Cell()
    {
        possibleTiles = new List<Tile>();
        impossibleTiles = new List<Tile>();
        gridCoords = Vector2.zero;
    }

    public Cell(List<Tile> possibleTiles, List<Tile> impossibleTiles, Vector2 gridCoords)
    {
        this.possibleTiles = possibleTiles;
        this.impossibleTiles = impossibleTiles;
        this.gridCoords = gridCoords;
    }
}
