using UnityEngine;
using System.Collections;

public interface IRotationControls
{
    Vector2 Sensivity { get; set; }

    Vector3 Rotation { get; set; }

    Transform Target { get; }
}
