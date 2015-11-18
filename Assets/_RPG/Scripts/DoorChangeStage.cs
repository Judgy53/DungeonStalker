using UnityEngine;
using System.Collections;

public class DoorChangeStage : MazeCellEdge
{
    [SerializeField]
    private uint delta = 1;
    public uint Delta { get { return delta; } set { delta = value; } }

    public Transform[] toColor = new Transform[0];

    public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        base.Initialize(cell, otherCell, direction);
        foreach (Transform t in toColor)
            t.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
    }

    public void Use()
    {
        GameManager.LoadStage(GameManager.Stage + delta);
    }
}
