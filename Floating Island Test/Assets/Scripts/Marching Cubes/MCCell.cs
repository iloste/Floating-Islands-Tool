using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCell
{

    /// <summary>
    /// the order of vertices for the cube starts 0 = bottom front left, 1 = bottom back left, 2 = bottom back right, 
    /// 3 = bottom front right. 4, 5, 6, 7 repeats this pattern but for the top.
    /// </summary>
    public MCVertex[] vertices;
    public Vector3Int coords;
    GameObject[] GOs;
    MCTile[] tiles;
    public MSVertex temp;

    public MCCell()
    {
        GOs = new GameObject[8];
        vertices = new MCVertex[8];
        tiles = new MCTile[8];
    }


    public void SetVertices(ref MCVertex vertex, int num)
    {
        vertices[num] = vertex;
    }


    /// <summary>
    /// Displays the given tiles if they differ from the tiles that already exist. 
    /// </summary>
    /// <param name="newTiles"></param>
    /// <param name="prefabs"></param>
    private void DisplayTiles(MCTile[] newTiles, GameObject[] prefabs)
    {
        if (newTiles.Length != tiles.Length)
        {
            Debug.LogError("Array Length Missmatch");
            return;
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != newTiles[i])
            {
                GameObject.Destroy(GOs[i]);
                tiles[i] = newTiles[i];

                if (tiles[i] != null && tiles[i].tileType != MCTile.TileType.None)
                {
                    Vector3 offset = GetOffset(i);

                    GOs[i] = GameObject.Instantiate(prefabs[(int)tiles[i].tileType - 1], new Vector3(vertices[i].coords.x, vertices[i].coords.z, vertices[i].coords.y) + offset, Quaternion.identity);
                    GOs[i].transform.eulerAngles = new Vector3(0, 90 * tiles[i].rotationIndex, 0);
                   // GOs[i].name = tiles[i].tileType.ToString();
                }
            }
        }
    }


    /// <summary>
    /// Returns the offset for a tile based on the vertex position
    /// </summary>
    /// <param name="vertexPos"></param>
    /// <returns></returns>
    private Vector3 GetOffset(int vertexPos)
    {
        Vector3 offset = Vector3.zero;

        switch (vertexPos)
        {
            case 0:
                offset = new Vector3(+.25f, .25f, +.25f);
                break;
            case 1:
                offset = new Vector3(+.25f, .25f, -.25f);
                break;
            case 2:
                offset = new Vector3(-.25f, .25f, -.25f);
                break;
            case 3:
                offset = new Vector3(-.25f, .25f, +.25f);
                break;
            case 4:
                offset = new Vector3(+.25f, -.25f, +.25f);
                break;
            case 5:
                offset = new Vector3(+.25f, -.25f, -.25f);
                break;
            case 6:
                offset = new Vector3(-.25f, -.25f, -.25f);
                break;
            case 7:
                offset = new Vector3(-.25f, -.25f, +.25f);
                break;
            default:
                offset = Vector3.zero;
                break;
        }

        return offset;
    }


    /// <summary>
    /// returns a bool[] for which vertices are full and which aren't
    /// </summary>
    /// <returns></returns>
    private bool[] GetVertices()
    {
        bool[] v = new bool[vertices.Length];

        for (int i = 0; i < v.Length; i++)
        {
            v[i] = vertices[i].full;
        }

        return v;
    }

    /// <summary>
    /// find out what's changed.
    /// delete the tiles that have changed.
    /// set the tiles that have changed.
    /// display the tiles that have changed.
    /// </summary>
    /// <param name="prefabs"></param>
    public void Update(GameObject[] prefabs)
    {
        bool[] v = GetVertices();
        MCTile[] newTiles = GetTileTypes(v);
        DisplayTiles(newTiles, prefabs);
    }



    MCTile[] GetTileTypes(bool[] vertices)
    {
        MCTile[] tileTypes = new MCTile[8];

        #region if all full or all empty
        //if (vertices[0] && vertices[1] && vertices[2] && vertices[3])
        //{
        //    tileTypes[0] = new MCTile(0, MCTile.TileType.Center);
        //    tileTypes[1] = new MCTile(0, MCTile.TileType.Center);
        //    tileTypes[2] = new MCTile(0, MCTile.TileType.Center);
        //    tileTypes[3] = new MCTile(0, MCTile.TileType.Center);
        //    return tileTypes;
        //}
        //else if (!vertices[0] && !vertices[1] && !vertices[2] && !vertices[3])
        //{
        //    tileTypes[0] = new MSTile(0, MSTile.TileType.None);
        //    tileTypes[1] = new MSTile(0, MSTile.TileType.None);
        //    tileTypes[2] = new MSTile(0, MSTile.TileType.None);
        //    tileTypes[3] = new MSTile(0, MSTile.TileType.None);
        //    return tileTypes;
        //}
        #endregion

        #region Bottom Corners
        for (int i = 0; i < 4; i++)
        {
            bool mainCorner = vertices[i];
            bool forwardCorner = vertices[(i + 1) % 4];
            bool rightCorner = vertices[(i + 3) % 4];
            bool diagonalCorner = vertices[(i + 2) % 4];
            bool upCorner = vertices[(i + 4) % 8];

            //if (mainCorner)// && (forwardCorner || rightCorner || upCorner))
            //{
            //    Debug.Log(coords);
            //}

            if (mainCorner && !forwardCorner && !rightCorner && !upCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerTop);
            }
            else if (mainCorner && !forwardCorner && !rightCorner && upCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerCenter);
            }
            else if (mainCorner && forwardCorner && !rightCorner && !upCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.EdgeTop);
            }
            else if (mainCorner && !forwardCorner && rightCorner && !upCorner)
            {
                tileTypes[i] = new MCTile((i - 1) % 4, MCTile.TileType.EdgeTop);
            }
            else if (mainCorner && forwardCorner && !rightCorner && upCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.EdgeCenter);
            }
            else if (mainCorner && !forwardCorner && rightCorner && upCorner)
            {
                tileTypes[i] = new MCTile((i - 1) % 4, MCTile.TileType.EdgeCenter);
            }
            else if (mainCorner && forwardCorner && rightCorner && !upCorner && diagonalCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.MiddleTop);
            }
            else if (mainCorner && forwardCorner && rightCorner && !upCorner && !diagonalCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerTopInverted);
            }
            else if (mainCorner && forwardCorner && rightCorner && upCorner && !diagonalCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerCenterInverted);
            }
        }
        #endregion

        if (coords == new Vector3Int(1, 0, 0))
        {

        }
        #region Top Corners
        for (int i = 4; i < 8; i++)
        {
            bool mainCorner = vertices[i];
            bool forwardCorner = vertices[4 + (i + 1) % 4];
            bool rightCorner = vertices[4 + (i + 3) % 4];
            bool diagonalCorner = vertices[4 + (i + 2) % 4];
            bool downCorner = vertices[(i + 4) % 8];

            //if (mainCorner)//&& (forwardCorner || rightCorner || downCorner))
            //{
            //    Debug.Log(coords);

            //}
            if (mainCorner && !forwardCorner && !rightCorner && !downCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerBottom);
            }
            else if (mainCorner && !forwardCorner && !rightCorner && downCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerCenter);
            }
            else if (mainCorner && forwardCorner && !rightCorner && !downCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.EdgeBottom);
            }
            else if (mainCorner && !forwardCorner && rightCorner && !downCorner)
            {
                tileTypes[i] = new MCTile((i - 1) % 4, MCTile.TileType.EdgeBottom);
            }
            else if (mainCorner && forwardCorner && !rightCorner && downCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.EdgeCenter);
            }
            else if (mainCorner && !forwardCorner && rightCorner && downCorner)
            {
                tileTypes[i] = new MCTile((i - 1) % 4, MCTile.TileType.EdgeCenter);
            }
            else if (mainCorner && forwardCorner && rightCorner && !downCorner && diagonalCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.MiddleBottom);
            }
            else if (mainCorner && forwardCorner && rightCorner && !downCorner && !diagonalCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerBottomInverted);
            }
            else if (mainCorner && forwardCorner && rightCorner && downCorner && !diagonalCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.CornerCenterInverted);
            }
        }
        #endregion

        return tileTypes;
    }
}
