using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovementEffect : IMovementEffect
{
    public EffectManager manager = null;
    public EffectManager Manager { get { return manager; } set { manager = value; } }

    [SerializeField]
    private string effectName = "MovementEffect";
    public string Name { get { return effectName; } }

    [SerializeField]
    private Sprite effectSprite = null;
    public Sprite EffectSprite { get { return effectSprite; } set { effectSprite = value; } }

    [SerializeField]
    private bool showSprite = true;
    public bool ShowSprite { get { return showSprite; } set { showSprite = value; } }

    [SerializeField]
    private float remainingTime = 5.0f;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [SerializeField]
    private bool isInfinite = false;
    public bool IsInfinite { get { return isInfinite; } set { isInfinite = value; } }

    [SerializeField]
    private EffectStyle style = EffectStyle.Buff;
    public EffectStyle Style { get { return style; } }

    [SerializeField]
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

    public void Update()
    {
    }

    public void OnApply(EffectManager manager)
    {
    }

    public void OnDestroy()
    {
    }

    public IEffect Clone()
    {
        IEffect effect = this.MemberwiseClone() as IEffect;
        effect.Manager = null;

        return effect;
    }
}
