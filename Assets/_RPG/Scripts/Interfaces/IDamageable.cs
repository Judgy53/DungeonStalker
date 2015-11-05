﻿using UnityEngine;
using System;
using System.Collections;

public interface IDamageable 
{
    event EventHandler OnKill;

    float Damage { get; }

    void AddDamage(float damages);
    void AddDamage(float damages, StatsManager other);

    bool WillKill(float damages);
    bool WillKill(float damages, StatsManager other);
}
