using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public TerrainType[] walkableRegions;
    public int obstacleProximityPenalty = 10;
    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    Node[,,] grid;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;
    int gridSizeZ;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        //define how many nodes there are in the grid in each axis
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY * gridSizeZ;
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        //find the bottom left point of the grid
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2 - Vector3.forward * gridWorldSize.z / 2;
        //loop through all axis and create a node at offset points to the bottom left of the grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    //get the world point of the current node
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    //spherecast to find if this node is in any unwalkable objects
                    bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);

                    int movementPenalty = 0;

                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }
                    //create the node
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z, movementPenalty);
                }
            }
        }
        BlurPenaltyMap(2);
    }

    private void BlurPenaltyMap(int blurSize)
    {
        int kernalSize = blurSize * 2 + 1;
        int kernalExtents = kernalSize - 1 / 2;

        int[,,] penaltiesXPass = new int[gridSizeX, gridSizeY, gridSizeZ];
        int[,,] penaltiesZPass = new int[gridSizeX, gridSizeY, gridSizeZ];
        int[,,] penaltiesYPass = new int[gridSizeX, gridSizeY, gridSizeZ];

        //Y axis blur pass (first blur pass)
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int x = -kernalExtents; x < kernalExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernalExtents);
                    penaltiesXPass[0, y, z] += grid[sampleX, y, z].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernalExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernalExtents, 0, gridSizeX - 1);

                    penaltiesXPass[x, y, z] = penaltiesXPass[x - 1, y, z] - grid[removeIndex, y, z].movementPenalty + grid[addIndex, y, z].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {

                for (int z = -kernalExtents; z < kernalExtents; z++)
                {
                    int sampleZ = Mathf.Clamp(z, 0, kernalExtents);
                    penaltiesZPass[x, y, 0] += penaltiesXPass[x, y, sampleZ];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesZPass[x, y, 0] / kernalSize * kernalSize);
                grid[x, y, 0].movementPenalty = blurredPenalty;

                for (int z = 1; z < gridSizeZ; z++)
                {
                    int removeIndex = Mathf.Clamp(z - kernalExtents - 1, 0, gridSizeZ);
                    int addIndex = Mathf.Clamp(z + kernalExtents, 0, gridSizeZ - 1);

                    penaltiesZPass[x, y, z] = penaltiesZPass[x, y, z - 1] - penaltiesXPass[x, y, removeIndex] + penaltiesXPass[x, y, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesZPass[x, y, z] / kernalSize * kernalSize);
                    grid[x, y, z].movementPenalty = blurredPenalty;
                }
            }
        }

        ////X axis blur pass(second pass)
        //for (int x = 0; x < gridSizeX; x++)
        //{
        //    for (int z = 0; z < gridSizeZ; z++)
        //    {
        //        for (int y = -kernalExtents; y < kernalExtents; y++)
        //        {
        //            int sampleY = Mathf.Clamp(y, 0, kernalExtents);
        //            penaltiesXPass[x, 0, z] += grid[x, sampleY, z].movementPenalty;
        //        }

        //        for (int y = 1; y < gridSizeY; y++)
        //        {
        //            int removeIndex = Mathf.Clamp(y - kernalExtents - 1, 0, gridSizeY);
        //            int addIndex = Mathf.Clamp(y + kernalExtents, 0, gridSizeY - 1);

        //            penaltiesXPass[x, y, z] = penaltiesXPass[x, y - 1, z] - grid[x, removeIndex, z].movementPenalty + grid[x, addIndex, z].movementPenalty;
        //        }
        //    }

        //    for (int y = 0; y < gridSizeY; y++)
        //    {
        //        for (int z = -kernalExtents; z < kernalExtents; z++)
        //        {
        //            int sampleZ = Mathf.Clamp(z, 0, kernalExtents);
        //            penaltiesZPass[x, y, 0] += penaltiesXPass[x, y, sampleZ];
        //        }

        //        for (int z = 1; z < gridSizeZ; z++)
        //        {
        //            int removeIndex = Mathf.Clamp(z - kernalExtents - 1, 0, gridSizeZ);
        //            int addIndex = Mathf.Clamp(z + kernalExtents, 0, gridSizeZ - 1);

        //            penaltiesZPass[x, y, z] = penaltiesZPass[x, y, z - 1] - penaltiesXPass[x, y, removeIndex] + penaltiesXPass[x, y, addIndex];
        //            int blurredPenalty = Mathf.RoundToInt((float)penaltiesZPass[x, y, z] / kernalSize * kernalSize);
        //            grid[x, y, z].movementPenalty = blurredPenalty;
        //        }
        //    }
        //}
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }
        return (neighbours);
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        //find the percent position of each axis in the grid
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z - transform.position.z + gridWorldSize.z / 2) / gridWorldSize.z;

        //clamp percent values between 0 and 1
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        //round to int
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[x, y, z];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridWorldSize);
        if (grid != null && displayGridGizmos)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}
