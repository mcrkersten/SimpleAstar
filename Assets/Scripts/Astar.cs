using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int start, Vector2Int target, Cell[,] grid)
    {
        Cell startCell = grid[start.x,start.y];
        Cell targetCell = grid[target.x, target.y]; 

        List<Cell> openList = new List<Cell>();
        HashSet<Cell> closedList = new HashSet<Cell>();
        openList.Add(startCell);

        while(openList.Count > 0)
        {
            Cell currentCell = openList[0];
            for (int i = 1; i < openList.Count; i++)
                if (openList[i].FScore < currentCell.FScore || openList[i].FScore == currentCell.FScore)
                    if (openList[i].HScore < currentCell.HScore)
                        currentCell = openList[i];

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            if (currentCell == targetCell)
                return RetracePath(startCell, targetCell);

            foreach (Cell neighbour in GetAccessibleNeighbours(currentCell, grid))
            {
                if (closedList.Contains(neighbour))
                    continue;

                float newCostToNeighbour = currentCell.GScore + 1;
                if (newCostToNeighbour < neighbour.GScore || !openList.Contains(neighbour))
                {
                    neighbour.GScore = newCostToNeighbour;
                    neighbour.HScore = 1;
                    neighbour.parent = currentCell;

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }

        List<Vector2Int> x = RetracePath(startCell, targetCell);
        Debug.Log(x.Count);
        return x;
    }

    private List<Cell> GetAccessibleNeighbours(Cell cell, Cell[,] grid)
    {
        List<Cell> result = new List<Cell>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int cellX = cell.gridPosition.x + x;
                int cellY = cell.gridPosition.y + y;
                if (cellX < 0 || cellX >= grid.GetLength(0) || cellY < 0 || cellY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
                    continue;

                Cell canditateCell = grid[cellX, cellY];
                if (IsAccesible(cell, canditateCell))
                    result.Add(canditateCell);
            }
        }
        return result;
    }

    private bool IsAccesible(Cell cell, Cell neighbour)
    {
        Vector2Int dirVector = cell.gridPosition - neighbour.gridPosition;
        if (dirVector.x != 0)
        {
            if (dirVector.x < 0)
                if(!cell.HasWall(Wall.RIGHT) && !neighbour.HasWall(Wall.LEFT))
                    return true;
            if (dirVector.x > 0)
                if (!cell.HasWall(Wall.LEFT) && !neighbour.HasWall(Wall.RIGHT))
                    return true;
        }
        if (dirVector.y != 0)
        {
            if (dirVector.y < 0)
                if (!cell.HasWall(Wall.UP) && !neighbour.HasWall(Wall.DOWN))
                    return true;
            if (dirVector.y > 0)
                if (!cell.HasWall(Wall.DOWN) && !neighbour.HasWall(Wall.UP))
                    return true;
        }

        return false;
    }

    List<Vector2Int> RetracePath(Cell startCell, Cell endCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = endCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }
        path.Reverse();

        List<Vector2Int> vectorPath = new List<Vector2Int>();
        foreach (Cell c in path)
            vectorPath.Add(c.gridPosition);
        return vectorPath;

    }
}
