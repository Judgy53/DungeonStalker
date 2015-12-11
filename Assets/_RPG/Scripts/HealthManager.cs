using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable, ISavable, IQuantifiable
{
    public event EventHandler OnDeath;
    public event EventHandler OnHit;

    [SerializeField]
    private float maxHealth = 10f;
    public float MaxHealth 
    { 
        get { return maxHealth; }
        set 
        {
            float ratio = currentHealth / maxHealth;
            maxHealth = value;
            currentHealth = maxHealth * ratio;
            CapHealth(); 
        } 
    }

    private float currentHealth = 10f;
    public float CurrentHealth 
    {
        get { return currentHealth; }
        set { currentHealth = value; CapHealth(); } 
    }

    public float Damage { get { return maxHealth - currentHealth; } }

    private EffectManager effectManager = null;

    private StatsManager statsManager = null;
    private ArmorManager armorManager = null;

    private void Awake()
    {
        effectManager = GetComponent<EffectManager>();

        statsManager = GetComponentInParent<StatsManager>();
        armorManager = GetComponentInParent<ArmorManager>();

        if (statsManager != null)
        { 
            statsManager.Stats.OnStatsChange += OnStatsChange;
            OnStatsChange(null, null); // get Max Health From Base Stats
        }

        currentHealth = maxHealth;
    }

    public void AddDamage(float damage)
    {
        if (currentHealth <= 0.0f)
            return;

        damage = ApplyDamagesModifiers(damage);

        CurrentHealth -= damage;

        if (OnHit != null)
            OnHit(this, new EventArgs());

        if (currentHealth <= 0f)
            Die();
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
        CurrentHealth += amount;
    }

    public void Die()
    {
        if (OnDeath != null)
            OnDeath(this, new EventArgs());
    }

    public void OnStatsChange(object sender, EventArgs args)
    {
        MaxHealth = statsManager.TotalStats.Stamina * 10u;
    }
	
    private float ComputeDamageReceived(StatsManager self, StatsManager other, float damages)
    {
        damages *= Mathf.Pow(0.95f, (float)self.Stats.Defense);

        float damageLevelModifier = ((float)other.CurrentLevel - (float)self.CurrentLevel) * 2.0f;
        float armorDamageReduction = 0.0f;
        if (armorManager != null)
        {
            int totalArmor = armorManager.TotalArmor;
            armorDamageReduction = ((float)totalArmor / (85.0f * (float)other.CurrentLevel + (float)totalArmor + 400.0f)) * 100.0f;
            armorDamageReduction = Mathf.Min(armorDamageReduction, 75.0f);
        }

        try
        {
            damages = checked(damages + damageLevelModifier);
        }
        catch
        {
            damages = 0.0f;
        }

        Debug.Log("Armor damage reduction : " + armorDamageReduction + "%");

        damages -= (armorDamageReduction / 100.0f) * damages;

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
	
    private void CapHealth()
    {
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void Save(SaveData data)
    {
        data.Add("MaxHealth", MaxHealth);
        data.Add("CurrentHealth", CurrentHealth);
    }

    public void Load(SaveData data)
    {
        MaxHealth = float.Parse(data.Get("MaxHealth"));
        CurrentHealth = float.Parse(data.Get("CurrentHealth"));

        if (currentHealth <= 0f)
            Die();
    }

    public float GetCurrentValue()
    {
        return currentHealth;
    }

    public float GetMaxValue()
    {
        return maxHealth;
    }
}