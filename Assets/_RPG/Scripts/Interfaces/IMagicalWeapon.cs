using UnityEngine;
using System.Collections;

public interface IMagicalWeapon : IWeapon
{
    float MinDamages { get; set; }
    float MaxDamages { get; set; }

    float MaxChargeTime { get; set; }
    float CurrentChargeTime { get; }

    MagicalWeaponUseState UseState { get; }
}

public enum MagicalWeaponUseState
{
    Default,
    Charging,
    Launching
}
