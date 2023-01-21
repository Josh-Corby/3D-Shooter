using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

[RequireComponent(typeof(Grid), typeof(PathRequestManager))]
public class Pathfinding : GameBehaviour
{
    PathRequestManager requestManager;
    private Grid grid;
    private int horizontalDistance;
    private int verticalDistance;
    private int totalDistance;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        //find respective nodes of start and end positions
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        if(startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    RetracePath(startNode, targetNode);
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }    
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return (waypoints);
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector3 directionOld = Vector3.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 directionNew = new Vector3(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY, path[i - 1].gridZ - path[i].gridZ);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        if (dstX > dstZ)
        {
            horizontalDistance = 14 * dstZ + 10 * (dstX - dstZ);
        }
        else
        {
            horizontalDistance = 14 * dstX + 10 * (dstZ - dstX);
        }

        if (dstX > dstY)
        {
            verticalDistance = 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            verticalDistance = 14 * dstX + 10 * (dstY - dstX);
        }

        totalDistance = verticalDistance + horizontalDistance;
        return (totalDistance);
    }
}
