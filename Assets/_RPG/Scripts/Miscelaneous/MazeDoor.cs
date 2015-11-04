using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage
{
    public Transform hinge = null;

    private MazeDoor OtherSideOfDoor
    {
        get
        {
            return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
        }
    }

    public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        base.Initialize(cell, otherCell, direction);
        if (OtherSideOfDoor != null)
        {
            if (hinge != null)
            {
                Vector3 hls = hinge.localScale;
                hinge.localScale = new Vector3(-hls.x, hls.y, hls.z);
                Vector3 p = hinge.localPosition;
                p.x = -p.x;
                hinge.localPosition = p;
            }
        }

        foreach (Transform c in transform)
        {
            if (c != hinge)
            {
                Renderer[] rends = c.GetComponentsInChildren<Renderer>();
                foreach (var r in rends)
                    r.material = cell.room.settings.wallMaterial;
            }
        }
    }
}
