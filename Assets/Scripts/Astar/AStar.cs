using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{

    private static Dictionary<Point, Node> nodes;


    private static void CreateNodes()
    {
       
        nodes = new Dictionary<Point, Node>();

        foreach (TileScript tile in LevelManager.Instance.Tiles.Values)
        {
            nodes.Add(tile.GridPosition, new Node(tile));
        }
    }

    public static Stack<Node> GetPath(Point start, Point goal)
    {
        if (nodes == null) 
        {
            CreateNodes();
        }
        
        //Creates an open list to be used with the A* algorithm
        HashSet<Node> openList = new HashSet<Node>();

        //Creates an closed list to be used with the A* algorithm
        HashSet<Node> closedList = new HashSet<Node>();

        // 1,2,3

        Stack<Node> finalPath = new Stack<Node>();

        //Finds the start node and creates a reference to it called current node
        Node currentNode = nodes[start];

        //1. Adds the start node to the OpenList
        openList.Add(currentNode);

        while (openList.Count > 0)//Step 10
        {
            //2. Runs through all neighbors 
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point neighbourPos = new Point(currentNode.GridPosition.X - x, currentNode.GridPosition.Y - y);

                    if (LevelManager.Instance.InBounds(neighbourPos) && LevelManager.Instance.Tiles[neighbourPos].Walkable && neighbourPos != currentNode.GridPosition)
                    {
                        //Sets the initial value of g to 0
                        int gCost = 0;

                        if (Math.Abs(x - y) == 1)//Check is we need to score 10
                        {
                            gCost = 10;
                        }
                        else //Scores 14 if we are diagonal
                        {
                            //Check if two nodes are connected diagonally
                            if (!ConnectedDiagonally(currentNode,nodes[neighbourPos]))
                            {
                                continue;
                            }
                            gCost = 14;
                        }

                        Node neighbour = nodes[neighbourPos];

                        if (openList.Contains(neighbour))
                        {
                            if (currentNode.G + gCost < neighbour.G)
                            {
                                neighbour.CalcValues(currentNode, nodes[goal], gCost);
                            }
                        }
                        else if (!closedList.Contains(neighbour))
                        {
                            openList.Add(neighbour); 
                            neighbour.CalcValues(currentNode, nodes[goal], gCost);
                        }

                    }
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (openList.Count > 0) { 
                
                currentNode = openList.OrderBy(n => n.F).First();
            }

            if (currentNode == nodes[goal])
            {
                while (currentNode.GridPosition != start)
                {
                    finalPath.Push(currentNode);
                    currentNode = currentNode.Parent;
                }

                break;
            }
        }


        return finalPath;
        //****THIS IS ONLY FOR DEBUGGING NEEDS TO BE REMOVED LATER!*****
       // GameObject.Find("AStarDebugger").GetComponent<AStarDebugger>().DebugPath(openList, closedList,finalPath);
    }

    private static bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        //Gets the direction to check
        Point direction = neighbor.GridPosition - currentNode.GridPosition;

        //The first node to check
        Point first = new Point(currentNode.GridPosition.X + direction.X, currentNode.GridPosition.Y);

        //The second node to check
        Point second = new Point(currentNode.GridPosition.X, currentNode.GridPosition.Y + direction.Y);

        //If they aren't connected diagonally we return false
        if (LevelManager.Instance.InBounds(first) && !LevelManager.Instance.Tiles[first].Walkable)
        {
            return false;
        }
        if (LevelManager.Instance.InBounds(second) && !LevelManager.Instance.Tiles[second].Walkable)
        {
            return false;
        }

        //We are allowed to move to the tile, because no towers are in the way.
        return true;
    }

}
