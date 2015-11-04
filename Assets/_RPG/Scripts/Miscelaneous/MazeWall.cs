using UnityEngine;
using System.Collections;

public class MazeWall : MazeCellEdge
{
    public Transform wall = null;

    public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        base.Initialize(cell, otherCell, direction);
        if (wall != null)
            wall.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
    }
}
