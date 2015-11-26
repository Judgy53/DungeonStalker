using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlockEffect : IDamageReceivedEffect
{
    private EffectManager manager = null;
    public EffectManager Manager { get { return manager; } set { manager = value; } }

    [SerializeField]
    private string name = "BlockEffect";
    public string Name { get { return name; } }

    [SerializeField]
    private Sprite effectSprite = null;
    public Sprite EffectSprite { get { return effectSprite; } set { effectSprite = value; } }

    [SerializeField]
    private bool showSprite = false;
    public bool ShowSprite { get { return showSprite; } set { showSprite = value; } }

    [SerializeField]
    private float remainingTime = 1.0f;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [SerializeField]
    private bool isInfinite = true;
    public bool IsInfinite { get { return isInfinite; } set { isInfinite = value; } }

    [SerializeField]
    private EffectStyle style = EffectStyle.Buff;
    public EffectStyle Style { get { return style; } }

    [SerializeField]
    private EffectType type = EffectType.Physical;
    public EffectType Type { get { return type; } }

    public float minBlockValue = 1.0f;
    public float maxBlockValue = 2.0f;

    public BlockEffect(float min, float max)
    {
        SetBlockValues(min, max);
    }

    public void SetBlockValues(float min, float max)
    {
        minBlockValue = min;
        maxBlockValue = max;
    }

    public float ApplyDamageModifier(ref float damages)
    {
        float rand = Random.Range(minBlockValue, maxBlockValue);
        damages = Mathf.Max(damages - rand, 0.0f);

        Debug.Log("Blocked " + rand + " damages ...");

        return damages;
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
