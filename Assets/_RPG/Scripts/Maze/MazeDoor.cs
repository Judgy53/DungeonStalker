﻿using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage
{
    public Transform hinge = null;

    private DoorState state = DoorState.Closed;
    public DoorState State { get { return state; } }

    private Animator animator = null;

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
        /*if (OtherSideOfDoor != null)
        {
            if (hinge != null)
            {
                Vector3 hls = hinge.localScale;
                hinge.localScale = new Vector3(-hls.x, hls.y, hls.z);
                Vector3 p = hinge.localPosition;
                p.x = -p.x;
                hinge.localPosition = p;
            }
        }*/

        foreach (Transform c in transform)
        {
            if (c != hinge)
            {
                Renderer[] rends = c.GetComponentsInChildren<Renderer>();
                foreach (var r in rends)
                    r.material = cell.room.settings.wallMaterial;
            }
        }

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("No AnimatorController on " + this.name);
    }

    public void ToogleState()
    {
        switch (state)
        {
            case DoorState.Closed:
                state = DoorState.Open;
                break;
            case DoorState.Open:
                state = DoorState.Closed;
                break;
        }

        if (animator != null)
            animator.SetBool("Open", state == DoorState.Open);
    }
}

public enum DoorState
{
    Closed,
    Open
}

public static class DoorStateExtensions
{
    public static DoorState GetOpposite(this DoorState state)
    {
        if (state == DoorState.Closed)
            return DoorState.Open;
        return DoorState.Closed;
    }

    public static string ToString(this DoorState state)
    {
        switch (state)
        {
            case DoorState.Closed:
                return "Closed";
            case DoorState.Open:
                return "Open";
        }

        throw new System.InvalidOperationException("Invalid state");
    }

    public static string ToActionString(this DoorState state)
    {
        switch (state)
        {
            case DoorState.Closed:
                return "Close";
            case DoorState.Open:
                return "Open";
        }

        throw new System.InvalidOperationException("Invalid state");
    }
}