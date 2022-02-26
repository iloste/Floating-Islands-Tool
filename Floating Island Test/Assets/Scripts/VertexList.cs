using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class VertexList : ScriptableObject
{
    public Vector3[] vertexPositions;
    public Vector3Int[] triplets;
    public Vector2[] uvs;
}
