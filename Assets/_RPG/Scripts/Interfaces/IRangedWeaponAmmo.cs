using UnityEngine;
using System.Collections;

public interface IRangedWeaponAmmo : ISavable
{
    event System.EventHandler OnOutOfAmmo;

    int AmmoLeft { get; set; }
    IRangedWeaponProjectile Projectile { get; }

    FloatMinMax AddedDamages { get; set; }

    IItem ItemPrefab { get; }

    IRangedWeaponProjectile InstantiateProjectile(Transform transform);
    void ApplyEffect(IRangedWeaponProjectile projectile);
    bool UseAmmo(int ammoConsumed);

    void TransferToContainer(IContainer container);
}
