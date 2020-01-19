/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using UnityEngine;


public class Geometry
{

    static public Vector3 PointFromGrid(Vector2Int gridPoint)
    {
        float x = gridPoint.x;
        float z= gridPoint.y;
        return new Vector3(x, 0, z);
    }

    static public Vector2Int intToVector2int(int index, int size)
    {
        int x = Mathf.FloorToInt(index / size);
        int y = index % size;
        return new Vector2Int(x, y);
    }

    static public Vector2Int GridPoint(int col, int row) => new Vector2Int(col, row);

    static public Vector3Int Cell2DTo3D(Vector2Int gridPoint) => new Vector3Int(gridPoint.x, gridPoint.y, 0);

    static public Vector3Int Cell2DTo3D(int row, int col) => new Vector3Int(row, col, 0);

    static public Vector2Int GridFromPoint(Vector3 point)
    {
        int col = Mathf.FloorToInt(4.0f + point.x);
        int row = Mathf.FloorToInt(4.0f + point.z);
        return new Vector2Int(col, row);
    }

    static public Vector2Int AdjacentCellTowardsGoalCell(Vector2Int startCell, Vector2Int goalCell)
    {
        if (startCell == goalCell) return new Vector2Int(0, 0);
        Vector2Int difference = goalCell - startCell;
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y)) // axe X prédominant
        {
            if (difference.x >= 0) return new Vector2Int(startCell.x + 1, startCell.y);
            else return new Vector2Int(startCell.x - 1, startCell.y);
        }
        else
        {
            if (difference.y >= 0) return new Vector2Int(startCell.x, startCell.y + 1);
            else return new Vector2Int(startCell.x, startCell.y - 1);
        }
    }

    static public Vector2Int ClosestCellToGoalCell(Vector2Int[] poolOfCells, Vector2Int goalCell)
    {
        Vector2Int closestCell = poolOfCells[0];
        int closestDistance = DistanceBetweenCells(closestCell, goalCell);
        for(int i = 1; i < poolOfCells.Length; i++)
        {
            Vector2Int cell = poolOfCells[i];
            int distance = DistanceBetweenCells(cell, goalCell);
            if(distance<closestDistance)
            {
                closestCell = cell;
                closestDistance = distance;
            }
        }
        return closestCell;
    }

    static public List<Vector2Int> CloserCellsToGoalCell(Vector2Int[] poolOfCells, Vector2Int goalCell, Vector2Int actualPos)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int actualDistance = DistanceBetweenCells(actualPos, goalCell);
        for (int i = 0; i < poolOfCells.Length; i++)
        {
            Vector2Int cell = poolOfCells[i];
            int distance = DistanceBetweenCells(cell, goalCell);
            if (distance <= actualDistance)
            {
                result.Add(cell);
            }
        }
        return result;
    }


    static public int DistanceBetweenCells(Vector2Int cellA, Vector2Int cellB)
    {
        Vector2Int difference = cellA - cellB;
        return Mathf.Abs(difference.x) + Mathf.Abs(difference.y);
    }
}


