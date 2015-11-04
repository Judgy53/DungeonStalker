using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject
{
    public int settingsIndex = 0;
    public MazeRoomSettings settings = null;

    private List<MazeCell> cells = new List<MazeCell>();

    public void Add(MazeCell cell)
    {
        cell.room = this;
        cells.Add(cell);
    }

    public void Assimilate(MazeRoom room)
    {
        foreach (MazeCell c in room.cells)
            Add(c);
    }
}
