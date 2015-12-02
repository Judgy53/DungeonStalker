using UnityEngine;
using System.Collections;

public interface IRangedWeaponProjectile
{
    IRangedWeapon Weapon { get; }

    float MinDamages { get; set; }
    float MaxDamages { get; set; }

    void Initialize(IRangedWeapon weapon);
}
