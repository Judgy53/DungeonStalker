using UnityEngine;
using System.Collections;

public class MazeCell : MonoBehaviour
{
    public Vector2i coordinates = new Vector2i(0,0);

    public MazeRoom room = null;

    public bool IsFullyInitialized { get { return initializedEdgeCount == MazeDirections.Count; } }

    public MazeDirection RandomUninitializedDirection
    {
        get
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                if (edges[i] == null)
                {
                    if (skips == 0)
                        return (MazeDirection)i;
                    skips--;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

    private int initializedEdgeCount = 0;

    public void Initialize(MazeRoom room)
    {
        room.Add(this);
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (var r in rends)
            r.material = room.settings.floorMaterial;
    }

    public MazeCellEdge GetEdge(MazeDirection direction)
    {
        return edges[(int)direction];
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
        edges[(int)direction] = edge;
        initializedEdgeCount++;
    }

    public void DestroyEdge(MazeDirection dir)
    {
        GameObject.Destroy(edges[(int)dir].gameObject);
        edges[(int)dir] = null;
        initializedEdgeCount--;
    }
}
