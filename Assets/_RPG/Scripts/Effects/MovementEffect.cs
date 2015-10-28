using UnityEngine;
using System.Collections;

public class MovementEffect : IMovementEffect
{
    private string effectName = "MovementEffect";
    public string Name { get { return effectName; } }

    private Sprite effectSprite = null;
    public Sprite EffectSprite { get { return effectSprite; } set { effectSprite = value; } }

    private bool showSprite = true;
    public bool ShowSprite { get { return showSprite; } set { showSprite = value; } }

    private float remainingTime = 5.0f;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    private bool isInfinite = false;
    public bool IsInfinite { get { return isInfinite; } set { isInfinite = value; } }

    private EffectStyle style = EffectStyle.Buff;
    public EffectStyle Style { get { return style; } }

    private EffectType type = EffectType.Magic;
    public EffectType Type { get { return type; } }

    public float movementMultiplier = 1.5f;

    public MovementEffect(string name, float multiplier, EffectStyle style, EffectType type)
    {
        effectName = name;
        movementMultiplier = multiplier;
        this.style = style;
        this.type = type;
    }

    public Vector3 ApplyMovementEffect(ref Vector3 movement)
    {
        movement *= movementMultiplier;
        return movement;
    }
}
