using UnityEngine;
using System.Collections;

public interface IRangedWeapon : IWeapon
{
    Transform FireMuzzle { get; }

    FloatMinMax BaseDamages { get; set; }

    bool UseAmmo { get; set; }

    int ConsumedAmmoPerShot { get; set; }

    float AnimationTime { get; set; }

    float ProjectileLifetime { get; set; }

    /// <summary>
    /// In degrees.
    /// </summary>
    float ProjectileDeviation { get; set; }

    int ProjectilePerShot { get; set; }

    void ProjectileHitCallback(IRangedWeaponProjectile p, IDamageable target, float damages);
    void ProjectileOnKillCallback(IRangedWeaponProjectile p, IDamageable target, float damages);
}

public enum RangedWeaponState
{
    Idle,
    Firing
}
