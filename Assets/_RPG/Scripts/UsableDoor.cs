using UnityEngine;
using System.Collections;

public class UsableDoor : MonoBehaviour, IUsable
{

    private bool closed = true;

    public string GetActionName()
    {
        if (closed)
            return "Open";
        else
            return "Close";
    }

    public string GetDescription()
    {
        return "Usable Door";
    }

    public void Use()
    {
        closed = !closed;
    }
}
