using UnityEngine;
using System.Collections;

public interface IControls
{
    Vector3 MoveSpeed { get; set; }

    CharacterController CC { get; }

    Vector3 Velocity { get; set; }
}
