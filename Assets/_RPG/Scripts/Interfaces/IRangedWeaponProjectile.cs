using UnityEngine;
using System.Collections;

public interface IRangedWeaponProjectile
{
    IRangedWeapon Weapon { get; }

    float MinDamages { get; set; }
    float MaxDamages { get; set; }

    Vector3 Direction { get; set; }

    void Initialize(IRangedWeapon weapon);

    void Hit(IDamageable damageable);
}
