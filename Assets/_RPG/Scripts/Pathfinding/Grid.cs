using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask = new LayerMask();
    public Vector2 gridWorldSize = Vector2.one;
    public float nodeRadius = 1.0f;
    public bool displayGrid = false;

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private Node[,] grid = new Node[0, 0];
    private Node[,] staticMask = new Node[0, 0];

    private float nodeDiameter = 0.0f;
    private int gridSizeX, gridSizeY;

    private static Grid instance = null;

    private bool processingObstacle = false;
    private Queue<ObstacleRecomputingRequest> obstacleRequests = new Queue<ObstacleRecomputingRequest>();
    private ObstacleRecomputingRequest currentRequest = null;

    private void Awake()
    {
        instance = this;

        nodeDiameter = nodeRadius * 2.0f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        staticMask = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2.0f - Vector3.forward * gridWorldSize.y / 2.0f;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);
                staticMask[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 wpos)
    {
        float percentX = (wpos.x + gridWorldSize.x / 2.0f) / gridWorldSize.x;
        float percentY = (wpos.z + gridWorldSize.y / 2.0f) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    /// <summary>
    /// This function will recompute the static nodes mask for the A* pathfinding.
    /// </summary>
    /// <param name="clearNodes">Shoud the function clear the grid ? (Warning : if set to true, dynamic obstacles will be cleared until they move again.)</param>
    public void RecomputeStaticObstacles(bool clearNodes = false)
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2.0f - Vector3.forward * gridWorldSize.y / 2.0f;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                if (!walkable || clearNodes)
                    grid[x, y].walkable = walkable;
                staticMask[x, y].walkable = walkable;
            }
        }
    }

    private void OnDrawGizmos()
    {
        nodeDiameter = nodeRadius * 2.0f;

        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, nodeDiameter, gridWorldSize.y));

        if (grid != null && displayGrid)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.05f));
            }
        }
    }

    public void RecomputeWalkableNode(Vector2i nodeCoordinates)
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2.0f - Vector3.forward * gridWorldSize.y / 2.0f;
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (nodeCoordinates.x * nodeDiameter + nodeRadius) + Vector3.forward * (nodeCoordinates.z * nodeDiameter + nodeRadius);

        grid[nodeCoordinates.x, nodeCoordinates.z].walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
    }

    public static bool EnqueueRecomputeObstacle(Bounds bounds, Node[] previouslyAffectedNodes, Action<Node[]> callback)
    {
        if (instance.obstacleRequests.Count < 20)
        {
            instance.obstacleRequests.Enqueue(new ObstacleRecomputingRequest(previouslyAffectedNodes, bounds, callback));
            instance.TryProcessNextRequest();
            return true;
        }

        return false;
    }

    private void TryProcessNextRequest()
    {
        if (!processingObstacle && obstacleRequests.Count > 0)
        {
            currentRequest = obstacleRequests.Dequeue();
            processingObstacle = true;
            StartCoroutine(RecomputeDynamicObstacleNodes(currentRequest.bounds, currentRequest.previouslyAffectedNodes));
        }
    }

    private void FinishedRecompute(Node[] affectedNodes)
    {
        currentRequest.callback(affectedNodes);
        processingObstacle = false;
        TryProcessNextRequest();
    }

    private IEnumerator RecomputeDynamicObstacleNodes(Bounds bounds, Node[] previouslyAffectedNodes)
    {
        foreach (var n in previouslyAffectedNodes)
            n.walkable = staticMask[n.gridX, n.gridY].walkable;

        List<Node> affectedNodes = new List<Node>();
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2.0f - Vector3.forward * gridWorldSize.y / 2.0f;
        foreach (Node node in grid)
        {
            if (!staticMask[node.gridX, node.gridY].walkable)
            {
                node.walkable = false;
                continue;
            }

            Vector3 worldPoint = worldBottomLeft + Vector3.right * (node.gridX * nodeDiameter + nodeRadius)
                + Vector3.forward * (node.gridY * nodeDiameter + nodeRadius);

            if (bounds.Intersects(new Bounds(worldPoint, new Vector3(nodeDiameter, nodeDiameter, nodeDiameter))))
            {
                RaycastHit[] hitInfo = Physics.SphereCastAll(worldPoint, nodeRadius, Vector3.one, 0.0f);
                if (hitInfo.Length == 0)
                    node.walkable = true;
                else
                {
                    Collider[] colliders = new Collider[hitInfo.Length];
                    for (int i = 0; i < hitInfo.Length; i++)
                        colliders[i] = hitInfo[i].collider;

                    node.walkable = true;
                    foreach (Collider c in colliders)
                    {
                        if (c.gameObject.GetComponentInParent<DynamicObstacle>() != null)
                        {
                            node.walkable = false;
                            affectedNodes.Add(node);
                            continue;
                        }
                    }
                }
            }
        }

        yield return null;

        FinishedRecompute(affectedNodes.ToArray());
    }
}

public class ObstacleRecomputingRequest
{
    public Node[] previouslyAffectedNodes;
    public Bounds bounds;
    public Action<Node[]> callback;

    public ObstacleRecomputingRequest(Node[] prev, Bounds b, Action<Node[]> callback)
    {
        previouslyAffectedNodes = prev;
        bounds = b;
        this.callback = callback;
    }
}