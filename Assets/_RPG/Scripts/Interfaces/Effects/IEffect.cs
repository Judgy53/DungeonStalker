using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public interface IEffect : ISavable
{
    EffectManager Manager { get; set; }

    string Name { get; }

    Sprite EffectSprite { get; set; }
    bool ShowSprite { get; set; }

    float RemainingTime { get; set; }
    bool IsInfinite { get; set; }

    void OnApply(EffectManager manager);
    void OnDestroy();
    void Update();

    IEffect Clone();

    EffectStyle Style { get; }
    EffectType Type { get; }

    bool ShouldBeSaved { get; set; }
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
