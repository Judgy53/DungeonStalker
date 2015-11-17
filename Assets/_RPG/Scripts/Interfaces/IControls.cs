using UnityEngine;
using System.Collections;

public interface IControls
{
    Vector3 MoveSpeed { get; set; }

    Vector3 Velocity { get; set; }

    bool IsGrounded { get; }
}
