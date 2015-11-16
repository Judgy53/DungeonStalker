using UnityEngine;
using System.Collections;

public class DoorButton : UsableButton
{
    public MazeDoor door = null;

    public new string GetActionName()
    {
        return door.State.GetOpposite().ToActionString() + " Door";
    }

    public new string GetDescription()
    {
        return "";
    }
}
