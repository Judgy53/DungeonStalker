using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : IHeapItem<Node>
{
    public bool walkable = true;
    public Vector3 worldPosition = Vector3.zero;

    public int gridX, gridY;

    public int gCost = 0;
    public int hCost = 0;

    public Node parent = null;

    private int heapIndex = 0;
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    public int FCost
    {
        get { return gCost + hCost; }
    }

    public Node(bool walkable, Vector3 wpos, int x, int y)
    {
        this.walkable = walkable;
        worldPosition = wpos;
        gridX = x;
        gridY = y;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
            compare = hCost.CompareTo(other.hCost);

        return -compare;
    }
}
