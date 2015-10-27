using UnityEngine;
using System.Collections;
using System;

public interface IDamageReceivedEffect : IEffect
{
    float ApplyDamageModifier(ref float damages);
}
