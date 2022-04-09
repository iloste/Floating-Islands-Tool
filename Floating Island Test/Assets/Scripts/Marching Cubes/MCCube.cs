using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCube
{

    /// <summary>
    /// the order of vertices for the cube starts 0 = bottom front left, 1 = bottom back left, 2 = bottom back right, 
    /// 3 = bottom front right. 4, 5, 6, 7 repeats this pattern but for the top.
    /// </summary>
    public MCVertex[] vertices;
    public MCCell[] cells;
    GameObject[] GOs;
    public Vector3Int coords;

    public MCCube()
    {
        Initialise();
    }


    private void Initialise()
    {
        GOs = new GameObject[8];
        vertices = new MCVertex[8];
    }


    public void SetVertices(ref MCVertex vertex, int num)
    {
        if (num >= 0 && num < vertices.Length)
        {
            vertices[num] = vertex;
        }
    }


    /// <summary>
    /// Returns the offset for a tile based on the vertex position
    /// </summary>
    /// <param name="vertexPos"></param>
    /// <returns></returns>
    private Vector3 GetOffset(int vertexPos)
    {
        Vector3 offset;

        //return Vector3.zero;

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
    private bool[] GetFilledVertices()
    {
        bool[] v = new bool[vertices.Length];

        for (int i = 0; i < v.Length; i++)
        {
            v[i] = vertices[i].full;
        }

        return v;
    }


    public void UpdateCubesCells()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            cells[i].SetTileExists(vertices[i].full);
        }

        FillCellSockets();
    }



    public void Update2()
    {
        for (int i = 0; i < GOs.Length; i++)
        {
            if (GOs[i] != null)
            {
                GameObject.Destroy(GOs[i]);
            }
        }

        bool[] v = GetFilledVertices();
        MCTile[] newTiles = GetTileTypes(v);


        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].coords == new Vector3Int(3, 2, 2))
            {

            }
            if (cells[i].TileExists)
            {
                if (cells[i].possibleTiles.Count > 0)
                {
                    Vector3 offset = GetOffset(i);

                    GOs[i] = GameObject.Instantiate(cells[i].possibleTiles[0].prefab, new Vector3(vertices[i].coords.x, vertices[i].coords.y, vertices[i].coords.z) + offset, Quaternion.identity);
                    GOs[i].transform.eulerAngles = new Vector3(0, 90 * cells[i].possibleTiles[0].rotationIndex, 0);
                    GOs[i].name = cells[i].possibleTiles[0].name + cells[i].coords;
                }// GOs[i].name = tiles[i].tileType.ToString();
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


    /// <summary>
    /// if N/E/S/W are true, up and down are false
    /// </summary>
    /// <param name="filledSockets"></param>
    private bool[] MiddleAmmendments(bool[] filledSockets, int pos)
    {
        if (vertices[0].full && vertices[1].full && vertices[2].full && vertices[3].full)
        {
            filledSockets[4] = false;
            filledSockets[5] = false;
        }

        if (vertices[4].full && vertices[5].full && vertices[6].full && vertices[7].full)
        {
            filledSockets[4] = false;
            filledSockets[5] = false;
        }

        return filledSockets;
    }


    private MCCell.MCValidConnection[] MiddleAmmendments(MCCell.MCValidConnection[] filledSockets, int pos)
    {
        if (vertices[1].full && vertices[2].full && vertices[5].full && vertices[6].full)
        {
            filledSockets[0] = MCCell.MCValidConnection.Middle;
        }

        if (vertices[2].full && vertices[3].full && vertices[6].full && vertices[7].full)
        {
            filledSockets[1] = MCCell.MCValidConnection.Middle;
        }

        if (vertices[0].full && vertices[3].full && vertices[4].full && vertices[7].full)
        {
            filledSockets[2] = MCCell.MCValidConnection.Middle;
        }

        if (vertices[0].full && vertices[1].full && vertices[4].full && vertices[5].full)
        {
            filledSockets[3] = MCCell.MCValidConnection.Middle;
        }

        if (vertices[0].full && vertices[1].full && vertices[2].full && vertices[3].full)
        {
           // filledSockets[4] = MCCell.MCValidConnection.Nothing;
            filledSockets[5] = MCCell.MCValidConnection.Middle;
        }

        if (vertices[4].full && vertices[5].full && vertices[6].full && vertices[7].full)
        {
            filledSockets[4] = MCCell.MCValidConnection.Middle;
           // filledSockets[5] = MCCell.MCValidConnection.Nothing;
        }

        //if (vertices[0].full && vertices[1].full && vertices[2].full && vertices[3].full && vertices[4].full && vertices[5].full && vertices[6].full && vertices[7].full)
        //{
        //    filledSockets[0] = MCCell.MCValidConnection.Middle;
        //    filledSockets[1] = MCCell.MCValidConnection.Middle;
        //    filledSockets[2] = MCCell.MCValidConnection.Middle;
        //    filledSockets[3] = MCCell.MCValidConnection.Middle;
        //    filledSockets[4] = MCCell.MCValidConnection.Middle;
        //    filledSockets[5] = MCCell.MCValidConnection.Middle;
        //}

        return filledSockets;
    }


    private void FillCellSockets()
    {
        bool[] filledSockets = new bool[6];

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].coords == new Vector3Int(3, 2, 2))
            {

            }
        }



        // represents the neighbours available to each cell. 
        // filledsockets is N/E/S/W/U/D.
        // if the given vertex is full, there is a neighbour.

        #region fill cell sockets
        // only fill if the tile exists
        // cell 0
        if (vertices[0].full)
        {
            filledSockets[0] = vertices[1].full;
            filledSockets[1] = vertices[3].full;
            filledSockets[2] = vertices[0].full;
            filledSockets[3] = vertices[0].full;
            filledSockets[4] = vertices[4].full;
            filledSockets[5] = vertices[0].full;
            filledSockets = MiddleAmmendments(filledSockets, 0);
            cells[0].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[0].SetValidConnections(new bool[6]);
        }

        // cell 1
        if (vertices[1].full)
        {
            filledSockets[0] = vertices[1].full;
            filledSockets[1] = vertices[2].full;
            filledSockets[2] = vertices[0].full;
            filledSockets[3] = vertices[1].full;
            filledSockets[4] = vertices[5].full;
            filledSockets[5] = vertices[1].full;
            filledSockets = MiddleAmmendments(filledSockets, 1);
            cells[1].SetValidConnections(filledSockets);
            filledSockets = new bool[6];

        }
        else
        {
            cells[1].SetValidConnections(new bool[6]);
        }

        // cell 2
        if (vertices[2].full)
        {
            filledSockets[0] = vertices[2].full;
            filledSockets[1] = vertices[2].full;
            filledSockets[2] = vertices[3].full;
            filledSockets[3] = vertices[1].full;
            filledSockets[4] = vertices[6].full;
            filledSockets[5] = vertices[2].full;
            filledSockets = MiddleAmmendments(filledSockets, 2);
            cells[2].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[2].SetValidConnections(new bool[6]);
        }

        // cell 3
        if (vertices[3].full)
        {
            filledSockets[0] = vertices[2].full;
            filledSockets[1] = vertices[3].full;
            filledSockets[2] = vertices[3].full;
            filledSockets[3] = vertices[0].full;
            filledSockets[4] = vertices[7].full;
            filledSockets[5] = vertices[3].full;
            filledSockets = MiddleAmmendments(filledSockets, 3);
            cells[3].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[3].SetValidConnections(new bool[6]);
        }

        // cell 4
        if (vertices[4].full)
        {
            filledSockets[0] = vertices[5].full;
            filledSockets[1] = vertices[7].full;
            filledSockets[2] = vertices[4].full;
            filledSockets[3] = vertices[4].full;
            filledSockets[4] = vertices[4].full;
            filledSockets[5] = vertices[0].full;
            filledSockets = MiddleAmmendments(filledSockets, 4);
            cells[4].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[4].SetValidConnections(new bool[6]);
        }

        // cell 5
        if (vertices[5].full)
        {
            filledSockets[0] = vertices[5].full;
            filledSockets[1] = vertices[6].full;
            filledSockets[2] = vertices[4].full;
            filledSockets[3] = vertices[5].full;
            filledSockets[4] = vertices[5].full;
            filledSockets[5] = vertices[1].full;
            filledSockets = MiddleAmmendments(filledSockets, 5);
            cells[5].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[5].SetValidConnections(new bool[6]);
        }

        // cell 6
        if (vertices[6].full)
        {
            filledSockets[0] = vertices[6].full;
            filledSockets[1] = vertices[6].full;
            filledSockets[2] = vertices[7].full;
            filledSockets[3] = vertices[5].full;
            filledSockets[4] = vertices[6].full;
            filledSockets[5] = vertices[2].full;
            filledSockets = MiddleAmmendments(filledSockets, 6);
            cells[6].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[6].SetValidConnections(new bool[6]);
        }

        // cell 7
        if (vertices[7].full)
        {
            filledSockets[0] = vertices[6].full;
            filledSockets[1] = vertices[7].full;
            filledSockets[2] = vertices[7].full;
            filledSockets[3] = vertices[4].full;
            filledSockets[4] = vertices[7].full;
            filledSockets[5] = vertices[3].full;
            filledSockets = MiddleAmmendments(filledSockets, 7);
            cells[7].SetValidConnections(filledSockets);
            filledSockets = new bool[6];
        }
        else
        {
            cells[7].SetValidConnections(new bool[6]);
        }

        #endregion
        #region fill cell sockets 2

        MCCell.MCValidConnection[] filledSockets2 = new MCCell.MCValidConnection[6];
        // only fill if the tile exists
        // cell 0
        if (vertices[0].full)
        {
            //condition ? 12 : null;
            filledSockets2[0] = vertices[1].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[3].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[0].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[0].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[4].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[0].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 0);
            cells[0].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[0].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 1
        if (vertices[1].full)
        {
            filledSockets2[0] = vertices[1].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[2].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[0].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[1].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[5].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[1].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 1);
            cells[1].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];

        }
        else
        {
            cells[1].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 2
        if (vertices[2].full)
        {
            filledSockets2[0] = vertices[2].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[2].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[3].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[1].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[6].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[2].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 2);
            cells[2].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[2].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 3
        if (vertices[3].full)
        {
            filledSockets2[0] = vertices[2].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[3].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[3].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[0].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[7].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[3].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 3);
            cells[3].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[3].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 4
        if (vertices[4].full)
        {
            filledSockets2[0] = vertices[5].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[7].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[4].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[4].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[4].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[0].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 4);
            cells[4].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[4].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 5
        if (vertices[5].full)
        {
            filledSockets2[0] = vertices[5].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[6].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[4].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[5].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[5].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[1].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 5);
            cells[5].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[5].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 6
        if (vertices[6].full)
        {
            filledSockets2[0] = vertices[6].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[6].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[7].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[5].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[6].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[2].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 6);
            cells[6].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[6].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        // cell 7
        if (vertices[7].full)
        {
            filledSockets2[0] = vertices[6].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[1] = vertices[7].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[2] = vertices[7].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[3] = vertices[4].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[4] = vertices[7].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2[5] = vertices[3].full ? MCCell.MCValidConnection.Something : MCCell.MCValidConnection.Nothing;
            filledSockets2 = MiddleAmmendments(filledSockets2, 7);
            cells[7].SetValidConnections(filledSockets2);
            filledSockets2 = new MCCell.MCValidConnection[6];
        }
        else
        {
            cells[7].SetValidConnections(new MCCell.MCValidConnection[6]);
        }

        #endregion

    }
}
