﻿using UnityEngine;
using System.Collections;
using System;

public interface IWeapon
{
    event EventHandler<OnHitArgs> OnHit;

    event EventHandler OnPrimary;
    event EventHandler OnEndPrimary;
    event EventHandler OnSecondary;
    event EventHandler OnEndSecondary;

    event EventHandler<OnKillArgs> OnKill;

    Vector3 HandPositionOffset { get; }
    Vector3 HandRotationOffset { get; }

    CharStats GearStats { get; }

    void Primary();
    void EndPrimary();
    void Secondary();
    void EndSecondary();

    void OnEquip();
    void OnUnequip();

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
    Magic,
    None
}

public class OnKillArgs : EventArgs
{
    public IDamageable target = null;

    public OnKillArgs(IDamageable damageable)
    {
        target = damageable;
    }
}

public class OnHitArgs : EventArgs
{
    public IDamageable target = null;
    public float damages = 0.0f;

    public OnHitArgs(IDamageable damageable, float dmg)
    {
        target = damageable;
        damages = dmg;
    }
}