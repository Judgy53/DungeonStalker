using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public interface IEffect
{
    string Name { get; }

    Sprite EffectSprite { get; set; }
    bool ShowSprite { get; set; }

    float RemainingTime { get; set; }
    bool IsInfinite { get; set; }

    EffectStyle Style { get; }
    EffectType Type { get; }
}

public enum EffectStyle
{
    Buff,
    Debuff
}

public enum EffectType
{
    Physical,
    Magic,
    Poison
}
