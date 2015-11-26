using UnityEngine;
using System.Collections;

[System.Serializable]
public class DotEffect : ScriptableObject, IMovementEffect, IDamageReceivedEffect
{
    private EffectManager manager = null;
    public EffectManager Manager { get { return manager; } set { manager = value; } }

    [SerializeField]
    private string effectName = "DotEffect";
    public string Name { get { return effectName; } }

    [SerializeField]
    private Sprite effectSprite = null;
    public Sprite EffectSprite { get { return effectSprite; } set { effectSprite = value; } }

    [SerializeField]
    private bool showSprite = false;
    public bool ShowSprite { get { return showSprite; } set { showSprite = value; } }

    [SerializeField]
    private float remainingTime = 12.0f;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [SerializeField]
    private bool isInfinite = false;
    public bool IsInfinite { get { return isInfinite; } set { isInfinite = value; } }

    [SerializeField]
    private EffectStyle style = EffectStyle.Debuff;
    public EffectStyle Style { get { return style; } }

    [SerializeField]
    private EffectType type = EffectType.Poison;
    public EffectType Type { get { return type; } }

    public float tickTime = 1.0f;
    public float minTickDamages = 1.0f;
    public float maxTickDamages = 1.0f;

    public float speedMultiplier = 1.0f;
    public float damageMultiplier = 1.0f;

    private float timer = 0.0f;

    private HealthManager healthManager = null;

    private OverlayManager overlayManager = null;

    public DotEffect(string name, float time, float tickTime, float minTickDamages, float maxTickDamages, EffectType type = EffectType.Poison)
    {
        this.name = name;
        this.remainingTime = time;
        this.tickTime = tickTime;
        this.minTickDamages = minTickDamages;
        this.maxTickDamages = maxTickDamages;
        this.type = type;
    }

    public Vector3 ApplyMovementEffect(ref Vector3 movement)
    {
        movement *= speedMultiplier;
        return movement;
    }

    public float ApplyDamageModifier(ref float damages)
    {
        damages *= damageMultiplier;
        return damages;
    }
    
    public void OnApply(EffectManager manager)
    {
        this.manager = manager;

        healthManager = manager.GetComponent<HealthManager>();

        if (manager.gameObject.tag == "Player")
        {
            overlayManager= manager.gameObject.GetComponentWithTag<OverlayManager>("OverlayManager");
            if (overlayManager != null && type == EffectType.Poison)
                overlayManager.RegisterPoisonOverlay();
        }
    }

    public void OnDestroy()
    {
        if (overlayManager != null && type == EffectType.Poison)
            overlayManager.UnregisterPoisonOverlay();
    }

    public void Update()
    {
        if (timer < tickTime)
        {
            timer += Time.deltaTime;
            return;
        }
        else
        {
            if (healthManager != null)
            {
                float damages = Random.Range(minTickDamages, maxTickDamages);
                healthManager.AddDamage(damages);
            }
            timer = 0.0f;
        }
    }

    public IEffect Clone()
    {
        IEffect effect = this.MemberwiseClone() as IEffect;
        effect.Manager = null;
        (effect as DotEffect).healthManager = null;

        return effect;
    }
}
