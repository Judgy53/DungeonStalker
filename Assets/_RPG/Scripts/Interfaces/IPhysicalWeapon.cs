using UnityEngine;
using System.Collections;
using System;

public interface IPhysicalWeapon : IWeapon
{
    float Damages { get; set; }
    float AttackSpeed { get; set; }
    float AnimationTime { get; set; }

    Transform StartRaycast { get; }
    Transform EndRaycast { get; }

    WeaponUseState UseState { get; }
}

public enum WeaponUseState
{
    Default,
    Attacking,
    Blocking
}
