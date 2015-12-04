using UnityEngine;
using System;
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

    [SerializeField]
    private bool shouldBeSaved = true;
    public bool ShouldBeSaved { get { return shouldBeSaved; } set { shouldBeSaved = value; } }

    public float movementMultiplier = 1.5f;

    public MovementEffect()
    {
        //Needed for reflection
    }

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

    public void Save(SaveData data)
    {
        data.Add("effectName", effectName);
        data.Add("remainingTime", remainingTime);

        data.Add("movementMultiplier", movementMultiplier);

        data.Add("style", style.ToString());
        data.Add("type", type.ToString());
    }

    public void Load(SaveData data)
    {
        effectName = data.Get("effectName");
        remainingTime = int.Parse(data.Get("remainingTime"));

        movementMultiplier = int.Parse(data.Get("movementMultiplier"));

        style = (EffectStyle)Enum.Parse(typeof(EffectStyle), data.Get("style"));
        type = (EffectType)Enum.Parse(typeof(EffectType), data.Get("type"));
    }
}
