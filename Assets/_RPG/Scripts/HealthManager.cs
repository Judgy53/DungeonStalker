﻿using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable, ISavable
{
    public event EventHandler OnKill;

    [SerializeField]
    private float maxHealth = 10f;
    public float MaxHealth 
    { 
        get { return maxHealth; }
        set { maxHealth = value; CapHealth(); } 
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

    private void Start()
    {
        currentHealth = maxHealth;
        effectManager = GetComponent<EffectManager>();

        statsManager = GetComponentInParent<StatsManager>();
        if (statsManager != null)
            statsManager.Stats.OnStatsChange += OnStatsChange;
    }

    public void AddDamage(float damage)
    {
        damage = ApplyDamagesModifiers(damage);

        CurrentHealth = CurrentHealth - damage;

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
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void Die()
    {
        if (OnKill != null)
            OnKill(this, new EventArgs());

        //Temporary
        Destroy(gameObject);
    }

    public void OnStatsChange(object sender, EventArgs args)
    {
        maxHealth = 50u + statsManager.Stats.Stamina * 30u;

        currentHealth = Mathf.Min(currentHealth, maxHealth);
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
    }
}