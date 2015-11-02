using UnityEngine;
using System.Collections;

public interface IMagicalWeapon : IWeapon
{
    float MinDamages { get; set; }
    float MaxDamages { get; set; }

    float MaxChargeTime { get; set; }
    float CurrentChargeTime { get; }

    float Cooldown { get; set; }

    float ManaCost { get; set; }
    float ManaChargeScale { get; set ; }
    float MaxManaCost { get; }

    MagicalWeaponUseState UseState { get; }
    MagicalWeaponType MagicalType { get; }
}

public enum MagicalWeaponUseState
{
    Default,
    Charging,
    Launching
}

public enum MagicalWeaponType
{
    Destruction,
    Restoration,
    Illusion
}
