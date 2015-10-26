﻿using UnityEngine;
using System.Collections;
using System;

public interface IWeapon
{
    event EventHandler OnHit;

    void Primary();
    void EndPrimary();
    void Secondary();
    void EndSecondary();

    WeaponHand WeaponHand { get; }
    WeaponRestriction WeaponRestrictions { get; }
    WeaponType WeaponType { get; }
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
