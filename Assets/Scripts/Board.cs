﻿/*
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectedMaterial;

    public Grid grid;

    public int size = 8;

    private Agent[,] occupedTiles;   // null value if the cell is not occuped. Otherwise it's the reference to the agent at thiss place.

    public GameObject tile;
    

    public Board()
    {
        occupedTiles = new Agent[size, size];
        
    }

    private void Awake()
    {

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                Color colorOfTile;
                if ((x + y) % 2 == 0) colorOfTile = Color.white;
                else colorOfTile = Color.black;

               
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = CoordToWorld(x,y);
                cube.transform.localScale = new Vector3(1f, 0.05f, 1f);
                cube.GetComponent<Renderer>().material.color = colorOfTile;

                //GameObject newPiece = Instantiate(tile, grid.CellToWorld(cellVector),
                //    Quaternion.identity, gameObject.transform);
            }
        }
    }

    // return the destination vector for the agent. change occupation status of occupedTiles
    public Vector3 AgentMoveTo(Agent movingAgent,Vector2Int destinationCell)
    {
        Vector3 targetPosition = grid.CellToWorld(Geometry.cell2DTo3D(destinationCell));
        if (destinationCell != movingAgent.actualPos)
        {
            if (occupedTiles[destinationCell.x, destinationCell.y] != null)
            {
                Debug.Log(destinationCell + " : " + occupedTiles[destinationCell.x, destinationCell.y]);
            }
            FreeCell(movingAgent.actualPos);
            occupedTiles[destinationCell.x, destinationCell.y] = movingAgent;
        }
        
        return targetPosition;
    }

    // return cells adjacent to agent and not obstructed by an other agent
    public List<Vector2Int> possibleMovesOfAgent(Agent agent)
    {
        List<Vector2Int> possibleCells = PossibleDirectionOfAgent(agent);
        possibleCells.RemoveAll(cell =>
        {
            return occupedTiles[cell.x, cell.y] != null;
        });

        
        return possibleCells;
    }

    // return cells adjacent to agent
    public List<Vector2Int> PossibleDirectionOfAgent(Agent agent)
    {
        List<Vector2Int> possibleCells = new List<Vector2Int>();
        Vector2Int pos = agent.actualPos;
        possibleCells.Add(pos);
        possibleCells.AddRange(AdjacentCells(agent.actualPos));
        return possibleCells;
    }

    public Agent isCellOccupied(Vector2Int cell)
    {
        return occupedTiles[cell.x, cell.y];
    }

    public List<Vector2Int>  AdjacentCells(Vector2Int cell)
    {
        List<Vector2Int> possibleCells = new List<Vector2Int>();
       
        for (int i = -1; i <= 1; i += 2)
        {
            int x = cell.x + i;
            int y = cell.y;
            if (x >= 0 && x < size && y >= 0 && y < size) // check for overbound
            {
                possibleCells.Add(new Vector2Int(x, y));
            }
        }

        for (int i = -1; i <= 1; i += 2)
        {
            int x = cell.x;
            int y = cell.y + i;
            if (x >= 0 && x < size && y >= 0 && y < size) // check for overbound
            {
                possibleCells.Add(new Vector2Int(x, y));
            }
        }
        return possibleCells;
    }

    public Agent getAnAdjacentAgent(Agent agent)
    {
        List<Vector2Int> adjacentCells =AdjacentCells(agent.actualPos);
        foreach(Vector2Int cell in adjacentCells)
        {
            if (occupedTiles[cell.x, cell.y] != null)
            {
                return occupedTiles[cell.x, cell.y];
            }
        }

        return null;
    }
        
    public void FreeCell(Vector2Int cell)
    {
        occupedTiles[cell.x, cell.y] = null;
    }

   

    public Vector3 CoordToWorld(int x,int y)
    {
        Vector3Int cellVector = Geometry.cell2DTo3D(x, y);
        return grid.CellToWorld(cellVector);
    }
    
    
    public void placeAgentOnCreation(Agent agent)
    {
        Vector2Int pos = agent.actualPos;
        occupedTiles[pos.x, pos.y] = agent;
    } 

    

    //public Dictionary<Vector2Int,int> updateValueOfPossibleMoves(Agent agent, Dictionary<Vector2Int, int> valueOfPossibleMoves)
    //{

    //}

    
}
