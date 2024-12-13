using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// A dictionary for all nodes in the game
    /// </summary>
    private static Dictionary<Point, Node> nodes;

    /// <summary>
    /// Creates a node for each tile in the game
    /// </summary>
    private static void CreateNodes()
    {
        //Instantiates the dicationary
        nodes = new Dictionary<Point, Node>();

        //Run throughm all the tiles in the game
        foreach (TileScript tile in LevelManager.Instance.Tiles.Values)
        {
            //Adds the node to the node dictionary
            nodes.Add(tile.GridPosition, new Node(tile));
        }
    }

    /// <summary>
    /// Generates a path with the A* algothithm
    /// </summary>
    /// <param name="start">The start of the path</param>
    public static Stack<Node> GetPath(Point start, Point goal)
    {
        if (nodes == null) //If we don't have nodes then we need to create them
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

        while (openList.Count > 0)  
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point neighbourPos = new Point(currentNode.GridPosition.X - x, currentNode.GridPosition.Y - y);

                    if (LevelManager.Instance.InBounds(neighbourPos) && LevelManager.Instance.Tiles[neighbourPos].Walkable && neighbourPos != currentNode.GridPosition)
                    {
                        
                        int gCost = 0;

                        if (Math.Abs(x - y) == 1)
                        {
                            gCost = 10;
                        }
                        else 
                        {
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

            if (openList.Count > 0)//STEP 7.
            {
                currentNode = openList.OrderBy(n => n.F).First();
            }

            if (currentNode == nodes[goal])
            {
                while (currentNode.GridPosition != start)
                {
                    finalPath.Push(currentNode);
                    currentNode = currentNode.Parent;
                }

               return finalPath;
            }
        }


        return null;
    }

    private static bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        Point direction = neighbor.GridPosition - currentNode.GridPosition;

        Point first = new Point(currentNode.GridPosition.X + direction.X, currentNode.GridPosition.Y);

        Point second = new Point(currentNode.GridPosition.X, currentNode.GridPosition.Y + direction.Y);

        if (LevelManager.Instance.InBounds(first) && !LevelManager.Instance.Tiles[first].Walkable)
        {
            return false;
        }
        if (LevelManager.Instance.InBounds(second) && !LevelManager.Instance.Tiles[second].Walkable)
        {
            return false;
        }

        return true;
    }

}
