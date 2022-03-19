using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSCell
{

    public MSVertex[] vertices;
    public Vector2Int coords;
    GameObject[] GOs;
    public MSVertex temp;

    public MSCell()
    {
        GOs = new GameObject[4];
        vertices = new MSVertex[4];
    }


    public void SetVertices(ref MSVertex vertex, int num)
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



        bool[] v = new bool[4];
        for (int i = 0; i < v.Length; i++)
        {
            v[i] = vertices[i].full;
        }

        MSTile[] tileTypes = GetTileTypes(v);

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (tileTypes[i] != null && tileTypes[i].tileType != MSTile.TileType.None)
            {
                Vector3 offset = Vector3.zero;

                switch (i)
                {
                    case 0:
                        //  offset = Vector3.zero;
                        offset = new Vector3(+.25f, 0, +.25f);
                        break;
                    case 1:
                        // offset = Vector3.zero;
                        //offset = new Vector3(.5f, 0, -.5f);
                        offset = new Vector3(+.25f, 0, -.25f);
                        break;
                    case 2:
                        //  offset = Vector3.zero;
                        offset = new Vector3(-.25f, 0, -.25f);
                        // offset = new Vector3(-.5f, 0, -.5f);
                        break;
                    case 3:
                        // offset = Vector3.zero;
                        offset = new Vector3(-.25f, 0, +.25f);
                        break;
                }
                // offset = Vector3.zero;

                GOs[i] = GameObject.Instantiate(prefabs[(int)tileTypes[i].tileType - 1], new Vector3(vertices[i].coords.x, 0, vertices[i].coords.y) + offset, Quaternion.identity);
                GOs[i].transform.eulerAngles = new Vector3(0, 90 * tileTypes[i].rotationIndex, 0);
                GOs[i].name = tileTypes[i].tileType.ToString();
            }
        }


        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    if (vertices[i].full)
        //    {
        //        Vector3 offset = Vector3.zero;

        //        switch (i)
        //        {
        //            case 0:
        //                offset = new Vector3(.5f, 0, .5f);
        //                break;
        //            case 1:
        //                offset = new Vector3(.5f, 0, -.5f);
        //                break;
        //            case 2:
        //                offset = new Vector3(-.5f, 0, -.5f);
        //                break;
        //            case 3:
        //                offset = new Vector3(-.5f, 0, .5f);
        //                break;
        //        }

        //        GOs[i] = GameObject.Instantiate(prefabs[i], new Vector3(vertices[i].coords.x, 0, vertices[i].coords.y) + offset, Quaternion.identity);
        //        GOs[i].transform.eulerAngles = new Vector3(0, 90 * prefabs[i].GetComponent<Test>().rotationIndex, 0);
        //        GOs[i].name = coords.ToString();
        //    }
        //}
    }



    MSTile[] GetTileTypes(bool[] vertices)
    {
        MSTile[] tileTypes = new MSTile[4];

        #region if all full or all empty
        if (vertices[0] && vertices[1] && vertices[2] && vertices[3])
        {
            tileTypes[0] = new MSTile(0, MSTile.TileType.Center);
            tileTypes[1] = new MSTile(0, MSTile.TileType.Center);
            tileTypes[2] = new MSTile(0, MSTile.TileType.Center);
            tileTypes[3] = new MSTile(0, MSTile.TileType.Center);
            return tileTypes;
        }
        else if (!vertices[0] && !vertices[1] && !vertices[2] && !vertices[3])
        {
            tileTypes[0] = new MSTile(0, MSTile.TileType.None);
            tileTypes[1] = new MSTile(0, MSTile.TileType.None);
            tileTypes[2] = new MSTile(0, MSTile.TileType.None);
            tileTypes[3] = new MSTile(0, MSTile.TileType.None);
            return tileTypes;
        }
        #endregion

        #region bottom left corner
        if (vertices[0] && !vertices[1] && !vertices[3])
        {
            tileTypes[0] = new MSTile(0, MSTile.TileType.Corner);
        }
        else if (vertices[0] && vertices[1] && !vertices[3])
        {
            tileTypes[0] = new MSTile(3, MSTile.TileType.Edge);
        }
        else if (vertices[0] && !vertices[1] && vertices[3])
        {
            tileTypes[0] = new MSTile(2, MSTile.TileType.InvertedEdge);
        }
        else if (vertices[0] && vertices[1] && vertices[3])
        {
            tileTypes[0] = new MSTile(2, MSTile.TileType.Center);
        }
        #endregion
        #region top left
        if (vertices[1] && !vertices[2] && !vertices[0])
        {
            tileTypes[1] = new MSTile(1, MSTile.TileType.Corner);
        }
        else if (vertices[1] && vertices[1] && !vertices[0])
        {
            tileTypes[1] = new MSTile(0, MSTile.TileType.Edge);
        }
        else if (vertices[1] && !vertices[2] && vertices[0])
        {
            tileTypes[1] = new MSTile(3, MSTile.TileType.InvertedEdge);
        }
        else if (vertices[1] && vertices[2] && vertices[0])
        {
            tileTypes[1] = new MSTile(3, MSTile.TileType.Center);
        }
        #endregion
        #region top right
        if (vertices[2] && !vertices[3] && !vertices[1])
        {
            tileTypes[2] = new MSTile(2, MSTile.TileType.Corner);
        }
        else if (vertices[2] && vertices[2] && !vertices[1])
        {
            tileTypes[2] = new MSTile(1, MSTile.TileType.Edge);
        }
        else if (vertices[2] && !vertices[3] && vertices[1])
        {
            tileTypes[2] = new MSTile(0, MSTile.TileType.InvertedEdge);
        }
        else if (vertices[2] && vertices[3] && vertices[1])
        {
            tileTypes[2] = new MSTile(0, MSTile.TileType.Center);
        }
        #endregion
        #region bottom right
        if (vertices[3] && !vertices[0] && !vertices[2])
        {
            tileTypes[3] = new MSTile(3, MSTile.TileType.Corner);
        }
        else if (vertices[3] && vertices[0] && !vertices[2])
        {
            tileTypes[3] = new MSTile(2, MSTile.TileType.Edge);
        }
        else if (vertices[3] && !vertices[0] && vertices[2])
        {
            tileTypes[3] = new MSTile(1, MSTile.TileType.InvertedEdge);
        }
        else if (vertices[3] && vertices[0] && vertices[2])
        {
            tileTypes[3] = new MSTile(1, MSTile.TileType.Center);
        }
        #endregion

        return tileTypes;
    }

}
