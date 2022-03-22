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

    public int rotationIndex;
    public TileType tileType;

    public MCTile(int rotationIndex, TileType tileType)
    {
        this.rotationIndex = rotationIndex;
        this.tileType = tileType;
    }
}
