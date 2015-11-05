using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable, ISavable
{
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
        if (effectManager != null)
        {
            IDamageReceivedEffect[] effects = effectManager.GetEffects<IDamageReceivedEffect>();
            foreach (var e in effects)
                e.ApplyDamageModifier(ref damage);
        }

        CurrentHealth = CurrentHealth - damage;

        if (currentHealth <= 0f)
            Die();

        UpdateBar();
    }

    public bool WillKill(float damages)
    {
        if (currentHealth - damages <= 0.0f)
            return true;
        return false;
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