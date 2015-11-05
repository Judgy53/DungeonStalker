using UnityEngine;
using System;
using System.Collections;

public class ManaManager : MonoBehaviour, ISavable
{

    [SerializeField]
    private float maxMana = 50.0f;
    public float MaxMana { get { return maxMana; } set { maxMana = value; } }

    private float currentMana = 50.0f;
    public float CurrentMana { get { return currentMana; } set { currentMana = value; } }

    [SerializeField]
    private float regenRate = 5.0f;
    public float RegenRate { get { return regenRate; } set { regenRate = value; } }

    [SerializeField]
    private UIBar uiBar = null;

    private StatsManager statsManager = null;

    private void Start()
    {
        currentMana = maxMana;

        statsManager = GetComponentInParent<StatsManager>();
        if (statsManager != null)
            statsManager.Stats.OnStatsChange += OnStatsChange;

        UpdateBar();
    }

    private void FixedUpdate()
    {
        currentMana += regenRate * Time.fixedDeltaTime;
        currentMana = Mathf.Min(currentMana, maxMana);

        UpdateBar();
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);

        UpdateBar();
    }

    public void RemoveMana(float amount)
    {
        currentMana = Mathf.Min(currentMana - amount, maxMana);

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (uiBar != null)
        {
            uiBar.CurrentValue = currentMana;
            uiBar.MaxValue = maxMana;
        }
    }

    public void OnStatsChange(object sender, EventArgs args)
    {
        maxMana = 40u + statsManager.Stats.Energy * 10u;
        regenRate = 4u + statsManager.Stats.Energy * 1u;

        currentMana = Mathf.Min(currentMana, maxMana);

        UpdateBar();
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
}
