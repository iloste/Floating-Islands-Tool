using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]

public class MCTile : ScriptableObject
{

    // written as type of piece (corner, edge, center), then top/bottom, then if inverted.
    public enum TileType
    {
        None,
        CornerBottom,
        CornerCenter,
        CornerTop,
        EdgeBottom,
        EdgeCenter,
        EdgeTop,
        MiddleBottom,
        MiddleTop,
        CornerBottomInverted,
        CornerTopInverted,
        ConerCenter,
        CornerCenterInverted,
    }


    public enum SpecialConnection
    {
        None,
        Flipped,
        Symmetrical,
        VerticalRotated0,
        VerticalRotated1,
        VerticalRotated2,
        VerticalRotated3,
    }


    public int rotationIndex;
    public TileType tileType;

    public Connection[] sockets;
    public GameObject prefab;

    [HideInInspector] public List<List<MCTile>> possibleConnectors;
    [HideInInspector] public List<List<MCTile>> immpossibleConnectors;

    public MCTile()
    {
        Initialise(0, TileType.None);
    }


    public MCTile(int rotationIndex, TileType tileType)
    {
        Initialise(rotationIndex, tileType);
    }


    private void Initialise(int rotationIndex, TileType tileType)
    {
        this.rotationIndex = rotationIndex;
        this.tileType = tileType;

        sockets = new Connection[6];
        possibleConnectors = new List<List<MCTile>>();

        for (int i = 0; i < sockets.Length; i++)
        {
            possibleConnectors.Add(new List<MCTile>());
        }
    }



  

}
