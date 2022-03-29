using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCInterfaceGrid : MonoBehaviour
{
   public MCVertexGrid vertexGrid;
   public MCCellGrid cellGrid;
   public MCCubeGrid cubeGrid;


    public MCInterfaceGrid(Vector3Int gridSize, List<MCTile> allTiles, GameObject vertexDebugPrefab, GameObject cellDebugPrefab,GameObject cubeDebugPrefab)
    {
        vertexGrid = new MCVertexGrid(gridSize, vertexDebugPrefab);
        cellGrid = new MCCellGrid(gridSize, allTiles, cellDebugPrefab);
        cubeGrid = new MCCubeGrid(gridSize, vertexGrid, cellGrid, cubeDebugPrefab);
    }


    #region Vertices
    public void FillVertex(Vector3Int coords)
    {
        vertexGrid.FillVertex(coords);
        cubeGrid.UpdateCubesCells();
        cellGrid.ResetPossibilitySpace();
    }


    public void ClearVertex(Vector3Int coords)
    {
        vertexGrid.ClearVertex(coords);
        cubeGrid.UpdateCubesCells();
    }


    public void DisplayVertices(bool displayVertices)
    {
        vertexGrid.DisplayVertices(displayVertices);
    }
    #endregion


    #region Cells
    //public void ResetPossibilitySpace()
    //{
    //    cellGrid.ResetPossibilitySpace();
    //}

    public void CollapsePossibleTiles(Vector3Int coords)
    {

        if (!cellGrid.GetCell(coords).Collapsed())
        {
            cellGrid.GetCell(coords).WaveFunctionCollapse(); 
        }
    }

    public MCCell GetCell(Vector3Int coords)
    {
        return cellGrid.GetCell(coords);
    }

    public MCCell[] GetNeighbouringCells(MCCell cell)
    {
        return cellGrid.GetNeighbouringCells(cell);
    }


    public bool GridCollapsed()
    {
        return cellGrid.GridCollapsed();
    }


    public MCCell FindUncollapsedCell()
    {
        return cellGrid.GetLowestEntropy();
    }
    #endregion


    #region Cubes
    //private void UpdateCubesCells()
    //{
    //    cubeGrid.UpdateCubesCells();
    //}

    public void DisplayCubes()
    {
        cubeGrid.DisplayCubes();
    }

    public void DisplayCubeLocations()
    {
        cubeGrid.DisplayCubes();
    }

    #endregion
}
