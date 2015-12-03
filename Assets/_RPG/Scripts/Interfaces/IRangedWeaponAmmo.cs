using UnityEngine;
using System.Collections;

public interface IRangedWeaponAmmo : IItem
{
    uint AmmoLeft { get; set; }

    void ApplyEffect(IRangedWeaponProjectile projectile);
    bool UseAmmo();
}
