using UnityEngine;
using System.Collections;

public class UsableDoor : MonoBehaviour, IUsable
{
    private Animator animator = null;

    private bool closed = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

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

    public void Use(InteractManager manager)
    {
        if(closed)
            animator.SetTrigger("DoorOpen");
        else
            animator.SetTrigger("DoorClose");

        closed = !closed;
    }
}
