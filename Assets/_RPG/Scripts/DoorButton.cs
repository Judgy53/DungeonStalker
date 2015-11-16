using UnityEngine;
using System.Collections;

public class DoorButton : UsableButton
{
    public MazeDoor door = null;

    public override string GetActionName()
    {
        return door.State.GetOpposite().ToActionString() + " Door";
    }

    public override string GetDescription()
    {
        return "";
    }
}
