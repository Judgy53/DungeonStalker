﻿using UnityEngine;
using System.Collections;
using System;

public interface IWeapon : ISavable
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

    bool CanFireContinuously { get; set; }

    void Primary();
    void EndPrimary();
    void Secondary();
    void EndSecondary();

    void OnEquip(WeaponManager manager);
    void OnUnequip();

    WeaponHand WeaponHand { get; }
    WeaponRestriction WeaponRestrictions { get; }
    WeaponType WeaponType { get; }

    GameObject InventoryItemPrefab { get; }

    void TransferToContainer(IContainer container);

    string GetInventoryDescription();

    bool AutoFirePrimaryClip { get; }
    bool StopPrimaryClipOnHit { get; }

    AudioClip GetPrimaryClip();
    AudioClip GetEndPrimaryClip();
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
    Axe = 1,
    Dagger = 2,
    Shield = 3,
    Staff = 4,
    Wand = 5,
    Magic = 6,
    Gun = 7,
    None = 8
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