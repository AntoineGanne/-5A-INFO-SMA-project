using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AStarPathFinder 
{
    private class Node
    {
        public Vector2Int pos;
        public int heuristic;

        public Node(Vector2Int pos, int heuristic)
        {
            this.pos = pos;
            this.heuristic = heuristic;
        }
    }

    static private int Compare(Node node1, Node node2)
    {
        if (node1.heuristic < node2.heuristic) return 1;
        else if (node1.heuristic == node2.heuristic) return 0;
        else return -1;
    }

    //static private Node[] FindPath(Agent[] occupedTiles, Node start, Node goal)
    //{
    //    Queue closedList = new Queue();
    //    List<Node> openList = new List<Node>();
    //    openList.Add(start);
    //    while (openList.Count > 0)
    //    {
    //        Node node=openList.
    //    }
    //}
}
