using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]

public class MSTile : ScriptableObject
{
    public enum TileType
    {
        None,
        Corner,
        Edge,
        InvertedCorner,
        InvertedEdge,
    }

    public int rotationIndex;
    public TileType tileType;

    public MSTile(int rotationIndex, TileType tileType)
    {
        this.rotationIndex = rotationIndex;
        this.tileType = tileType;
    }
}
