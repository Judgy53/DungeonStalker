﻿using UnityEngine;
using System;
using System.Collections;

public class ManaManager : MonoBehaviour, ISavable, IQuantifiable
{
    [SerializeField]
    private float maxMana = 50.0f;
    public float MaxMana { get { return maxMana; } set { maxMana = value; } }

    private float currentMana = 50.0f;
    public float CurrentMana 
    { 
        get { return currentMana; } 
        set 
        {
            float ratio = currentMana / maxMana;
            maxMana = value;
            currentMana = maxMana * ratio;
        } 
    }

    [SerializeField]
    private float regenRate = 5.0f;
    public float RegenRate { get { return regenRate; } set { regenRate = value; } }

    private StatsManager statsManager = null;

    private void Awake()
    {
        statsManager = GetComponentInParent<StatsManager>();
        if (statsManager != null)
        {
            statsManager.Stats.OnStatsChange += OnStatsChange;
            OnStatsChange(null, null); // get Max Mana From Base Stats
        }

        currentMana = maxMana;
    }

    private void FixedUpdate()
    {
        currentMana += regenRate * Time.fixedDeltaTime;
        currentMana = Mathf.Min(currentMana, maxMana);
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }

    public void RemoveMana(float amount)
    {
        currentMana = Mathf.Min(currentMana - amount, maxMana);
    }

    public void OnStatsChange(object sender, EventArgs args)
    {
        MaxMana = 40u + statsManager.TotalStats.Energy * 10u;
        regenRate = 4u + statsManager.TotalStats.Energy * 1u;

        currentMana = Mathf.Min(currentMana, maxMana);
    }

    public void Save(SaveData data)
    {
        data.Add("MaxMana", MaxMana);
        data.Add("CurrentMana", CurrentMana);
    }

    public void Load(SaveData data)
    {
        MaxMana = float.Parse(data.Get("MaxMana"));
        CurrentMana = float.Parse(data.Get("CurrentMana"));
    }

    public float GetCurrentValue()
    {
        return currentMana;
    }

    public float GetMaxValue()
    {
        return maxMana;
    }
}
