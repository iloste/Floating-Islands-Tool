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
    public MSVertex temp;

    public MCCell()
    {
        GOs = new GameObject[8];
        vertices = new MCVertex[8];
    }


    public void SetVertices(ref MCVertex vertex, int num)
    {
        vertices[num] = vertex;
    }


    public void Update(GameObject[] prefabs)
    {
        for (int i = 0; i < GOs.Length; i++)
        {
            if (GOs[i] != null)
            {
                GameObject.Destroy(GOs[i]);
            }
        }



        bool[] v = new bool[vertices.Length];
        for (int i = 0; i < v.Length; i++)
        {
            v[i] = vertices[i].full;
        }

        MCTile[] tileTypes = GetTileTypes(v);

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (tileTypes[i] != null && tileTypes[i].tileType != MCTile.TileType.None)
            {
                Vector3 offset = Vector3.zero;

                switch (i)
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
                }

                GOs[i] = GameObject.Instantiate(prefabs[(int)tileTypes[i].tileType - 1], new Vector3(vertices[i].coords.x, vertices[i].coords.z, vertices[i].coords.y) + offset, Quaternion.identity);
                GOs[i].transform.eulerAngles = new Vector3(0, 90 * tileTypes[i].rotationIndex, 0);
                GOs[i].name = tileTypes[i].tileType.ToString();
            }
        }
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
            bool upCorner = vertices[(i + 4) % 8];

            if (mainCorner)// && (forwardCorner || rightCorner || upCorner))
            {
                Debug.Log(coords);
            }

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
            else if (mainCorner && forwardCorner && rightCorner && !upCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.MiddleTop);
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
            bool downCorner = vertices[(i + 4) % 8];

            if (mainCorner)//&& (forwardCorner || rightCorner || downCorner))
            {
                Debug.Log(coords);

            }
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
            else if (mainCorner && forwardCorner && rightCorner && !downCorner)
            {
                tileTypes[i] = new MCTile(i, MCTile.TileType.MiddleBottom);
            }
        }
        #endregion

        return tileTypes;
        //#region bottom left front corner
        //if (vertices[0] && !vertices[1] && !vertices[3] && !vertices[4])
        //{
        //    tileTypes[0] = new MCTile(0, MCTile.TileType.CornerTop);
        //}
        ////else if (vertices[0] && vertices[1] && !vertices[3] && !vertices[4])
        ////{
        ////    tileTypes[0] = new MCTile(3, MCTile.TileType.EdgeTop);
        ////}
        //else if (vertices[0] && !vertices[1] && !vertices[3] && vertices[4])
        //{
        //    tileTypes[0] = new MCTile(2, MCTile.TileType.CornerCenter);
        //}
        ////else if (vertices[0] && vertices[1] && vertices[3])
        ////{
        ////    tileTypes[0] = new MSTile(2, MSTile.TileType.Center);
        ////}
        //#endregion
        //#region top left
        //if (vertices[1] && !vertices[2] && !vertices[0])
        //{
        //    tileTypes[1] = new MSTile(1, MSTile.TileType.Corner);
        //}
        //else if (vertices[1] && vertices[1] && !vertices[0])
        //{
        //    tileTypes[1] = new MSTile(0, MSTile.TileType.Edge);
        //}
        //else if (vertices[1] && !vertices[2] && vertices[0])
        //{
        //    tileTypes[1] = new MSTile(3, MSTile.TileType.InvertedEdge);
        //}
        //else if (vertices[1] && vertices[2] && vertices[0])
        //{
        //    tileTypes[1] = new MSTile(3, MSTile.TileType.Center);
        //}
        //#endregion
        //#region top right
        //if (vertices[2] && !vertices[3] && !vertices[1])
        //{
        //    tileTypes[2] = new MSTile(2, MSTile.TileType.Corner);
        //}
        //else if (vertices[2] && vertices[2] && !vertices[1])
        //{
        //    tileTypes[2] = new MSTile(1, MSTile.TileType.Edge);
        //}
        //else if (vertices[2] && !vertices[3] && vertices[1])
        //{
        //    tileTypes[2] = new MSTile(0, MSTile.TileType.InvertedEdge);
        //}
        //else if (vertices[2] && vertices[3] && vertices[1])
        //{
        //    tileTypes[2] = new MSTile(0, MSTile.TileType.Center);
        //}
        //#endregion
        //#region bottom right
        //if (vertices[3] && !vertices[0] && !vertices[2])
        //{
        //    tileTypes[3] = new MSTile(3, MSTile.TileType.Corner);
        //}
        //else if (vertices[3] && vertices[0] && !vertices[2])
        //{
        //    tileTypes[3] = new MSTile(2, MSTile.TileType.Edge);
        //}
        //else if (vertices[3] && !vertices[0] && vertices[2])
        //{
        //    tileTypes[3] = new MSTile(1, MSTile.TileType.InvertedEdge);
        //}
        //else if (vertices[3] && vertices[0] && vertices[2])
        //{
        //    tileTypes[3] = new MSTile(1, MSTile.TileType.Center);
        //}
        //#endregion

        //return tileTypes;
    }

}
