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

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void AddDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
            Die();
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