using UnityEngine;
using System.Collections;
using System;

public interface IWeapon
{
    event EventHandler OnHit;

    event EventHandler OnPrimary;
    event EventHandler OnEndPrimary;
    event EventHandler OnSecondary;
    event EventHandler OnEndSecondary;

    event EventHandler<OnKillArgs> OnKill;

    Vector3 HandPositionOffset { get; }
    Vector3 HandRotationOffset { get; }

    void Primary();
    void EndPrimary();
    void Secondary();
    void EndSecondary();

    void OnEquip();

    WeaponHand WeaponHand { get; }
    WeaponRestriction WeaponRestrictions { get; }
    WeaponType WeaponType { get; }

    GameObject InventoryItemPrefab { get; }

    void TransferToContainer(IContainer container);

    void ToSaveData(SaveData data, string name);
}

public enum WeaponHand : int
{
    OneHanded = 0,
    TwoHanded
}

public enum WeaponRestriction : int
{
    Both = 0,
    MainHand,
    OffHand
}

public enum WeaponType : int
{
    Sword = 0,
    Axe,
    Dagger,
    Shield,
    Staff,
    Wand,
    Magic
}

public class OnKillArgs : EventArgs
{
    public IDamageable target = null;

    public OnKillArgs(IDamageable damageable)
    {
        target = damageable;
    }
}
