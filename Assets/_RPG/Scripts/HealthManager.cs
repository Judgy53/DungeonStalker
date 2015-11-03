using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHealth = 10f;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    private float currentHealth = 10f;
    public float CurrentHealth { get { return currentHealth; } }

    public float Damage { get { return maxHealth - currentHealth; } }

    private EffectManager effectManager = null;

    private StatsManager statsManager = null;

    [SerializeField]
    private UIBar uiBar = null;

    private void Start()
    {
        currentHealth = maxHealth;
        effectManager = GetComponent<EffectManager>();

        statsManager = GetComponentInParent<StatsManager>();
        if (statsManager != null)
            statsManager.Stats.OnStatsChange += OnStatsChange;

        UpdateBar();
    }

    public void AddDamage(float damage)
    {
        damage = ApplyDamagesModifiers(damage);

        currentHealth = Mathf.Min(currentHealth - damage, maxHealth);

        if (currentHealth <= 0f)
            Die();

        UpdateBar();
    }

    public void AddDamage(float damages, StatsManager other)
    {
        float realDamages = ComputeDamageReceived(statsManager, other, damages);
        AddDamage(realDamages);
    }

    public bool WillKill(float damages)
    {
        float realDamages = ApplyDamagesModifiers(damages);

        if (currentHealth - realDamages <= 0.0f)
            return true;
        return false;
    }

    public bool WillKill(float damages, StatsManager other)
    {
        float realDamages = ComputeDamageReceived(statsManager, other, damages);
        return WillKill(realDamages);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (uiBar != null)
        {
            uiBar.CurrentValue = currentHealth;
            uiBar.MaxValue = maxHealth;
        }
    }

    public void Die()
    {
        //Temporary
        Destroy(gameObject);
    }

    public void OnStatsChange(object sender, EventArgs args)
    {
        maxHealth = 50u + statsManager.Stats.Stamina * 30u;

        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateBar();
    }

    private float ComputeDamageReceived(StatsManager self, StatsManager other, float damages)
    {
        damages *= Mathf.Pow(0.90f, (float)self.Stats.Defense);

        float damageLevelModifier = ((float)other.CurrentLevel - (float)self.CurrentLevel) * 2.0f;

        try
        {
            damages = checked(damages + damageLevelModifier);
        }
        catch
        {
            damages = 0.0f;
        }

        damages = Mathf.Max(damages, 0.1f);
        
        return damages;
    }

    private float ApplyDamagesModifiers(float damages)
    {
        if (effectManager != null)
        {
            IDamageReceivedEffect[] effects = effectManager.GetEffects<IDamageReceivedEffect>();
            foreach (var e in effects)
                e.ApplyDamageModifier(ref damages);
        }

        return damages;
    }
}