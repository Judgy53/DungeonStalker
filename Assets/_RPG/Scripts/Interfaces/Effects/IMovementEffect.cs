using UnityEngine;
using System.Collections;

public interface IMovementEffect : IEffect
{
    Vector3 ApplyMovementEffect(ref Vector3 movement);
}
