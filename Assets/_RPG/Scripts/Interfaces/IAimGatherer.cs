using UnityEngine;
using System.Collections;

public interface IAimGatherer
{
    Vector3 GetAimHitpoint(ref IDamageable dmg);
}
