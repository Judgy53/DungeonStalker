﻿using System;
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

    private void Start()
    {
        currentHealth = maxHealth;
        effectManager = GetComponent<EffectManager>();
    }

    public void AddDamage(float damage)
    {
        if (effectManager != null)
        {
            IDamageReceivedEffect[] effects = effectManager.GetEffects<IDamageReceivedEffect>();
            foreach (var e in effects)
                e.ApplyDamageModifier(ref damage);
        }

        currentHealth = Mathf.Min(currentHealth - damage, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public bool WillKill(float damages)
    {
        if (currentHealth - damages < 0.0f)
            return true;
        return false;
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void Die()
    {
        //Temporary
        Destroy(gameObject);
    }
}