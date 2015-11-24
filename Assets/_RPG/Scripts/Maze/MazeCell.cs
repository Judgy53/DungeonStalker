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

    private bool[] initializedCorners = new bool[4];

    public void Initialize(MazeRoom room)
    {
        room.Add(this);
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (var r in rends)
            r.material = room.settings.floorMaterial;
        for (int i = 0; i < initializedCorners.Length; i++)
            initializedCorners[i] = false;
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

    public bool IsCornerInitialized(MazeDirection dir, int delta)
    {
        return initializedCorners[(int)((float)dir + 0.5f + (float)delta / 2.0f) % MazeDirections.Count];
    }

    public void InitializeCorner(GameObject prefab, MazeDirection dir, int delta)
    {
        GameObject dgo = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        dgo.transform.parent = transform;
        dgo.transform.localPosition = Vector3.zero;
        dgo.transform.localRotation = dir.ToRotation();
        dgo.transform.localScale = new Vector3(delta, dgo.transform.localScale.y, dgo.transform.localScale.z);

        initializedCorners[(int)((float)dir + 0.5f + (float)delta / 2.0f) % MazeDirections.Count] = true;
    }
}
