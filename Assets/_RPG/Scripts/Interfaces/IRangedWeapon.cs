using UnityEngine;
using System.Collections;

public interface IRangedWeapon : IWeapon
{
    Transform FireMuzzle { get; }

    IRangedWeaponProjectile Projectile { get; }

    IRangedWeaponAmmo Ammo { get; set; }
    bool UseAmmo { get; set; }

    float AnimationTime { get; set; }

    float ProjectileLifetime { get; set; }

    void ProjectileHitCallback(IRangedWeaponProjectile p, IDamageable target, float damages);
    void ProjectileOnKillCallback(IRangedWeaponProjectile p, IDamageable target, float damages);
}

public enum RangedWeaponState
{
    Idle,
    Firing
}
