using System;
using UnityEngine;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour, ISavable
{
    public event EventHandler OnLevelUp;

    [SerializeField]
    private CharStats stats = new CharStats();
    public CharStats Stats { get { return stats; } set { stats = value; } }

    [SerializeField]
    private bool debugFireEvent = false;

    [SerializeField]
    private uint unspentPoint = 0u;
    public uint UnspentPoints { get { return unspentPoint; } set { unspentPoint = value; } }

    [SerializeField]
    private uint currentLevel = 1u;
    public uint CurrentLevel { get { return currentLevel; }
        set
        {
            currentLevel = value;
            currentExp = 0u;
            maxExp = ComputeMaxExp();
        }
    }
    
    [SerializeField]
    private uint currentExp = 0u;
    public uint CurrentExp { get { return currentExp; }
        set
        {
            if (!canGainExp)
                return;

            currentExp = value;
            if (currentExp >= maxExp)
            {
                currentLevel++;

                uint oldMaxExp = maxExp;

                maxExp = ComputeMaxExp();
                unspentPoint += 5u;

                if (OnLevelUp != null)
                    OnLevelUp(this, new EventArgs());

                CurrentExp = value - oldMaxExp;
                return;
            }

            if (xpBar != null)
            {
                xpBar.CurrentValue = (float)currentExp;
                xpBar.MaxValue = (float)MaxExp;
            }
        }
    }

    private uint maxExp = 100u;
    public uint MaxExp { get { return maxExp; } }

    public bool canGainExp = true;

    private WeaponManager weapManager = null;

    [SerializeField]
    private UIBar xpBar = null;

    //Fire this script's Start after every other Starts.
    private void Start()
    {
        stats.FireEvent();
        OnLevelUp += OnLevelUpCallback;

        weapManager = GetComponent<WeaponManager>();
        if (weapManager != null)
            weapManager.OnKill += OnKillCallback;

        maxExp = ComputeMaxExp();
    }

    private void OnDestroy()
    {
        if (weapManager != null)
            weapManager.OnKill -= OnKillCallback;
    }

    private void Update()
    {
        if (debugFireEvent)
        {
            stats.FireEvent();
            debugFireEvent = false;
        }
    }

    private void OnLevelUpCallback(object sender, EventArgs args)
    {
        IDamageable dmg = GetComponent<IDamageable>();
        dmg.AddDamage(-dmg.Damage);

        Debug.Log("Congratulations ! You reached level " + currentLevel + " !");
        Debug.Log("You have " + unspentPoint + " attributes point(s) to spend.");
    }

    private void OnKillCallback(object sender, OnKillArgs args)
    {
        StatsManager manager = (args.target as Behaviour).GetComponent<StatsManager>();
        if (manager != null && manager.currentLevel + 10 > currentLevel)
            CurrentExp += (manager.currentLevel * (uint)UnityEngine.Random.Range(5, 10));
    }

    private uint ComputeMaxExp()
    {
        return (uint)Mathf.RoundToInt(100 + Mathf.Pow(2.0f, (float)currentLevel));
    }

    public void Save(SaveData data)
    {
        data.Add("StatsUnspent", unspentPoint);

        data.Add("StatsStrength", Stats.Strength);
        data.Add("StatsDefense", Stats.Defense);
        data.Add("StatsStamina", Stats.Stamina);
        data.Add("StatsEnergy", Stats.Energy);

        data.Add("CurrentLevel", currentLevel);
        data.Add("CurrentExp", currentExp);
    }

    public void Load(SaveData data)
    {
        UnspentPoints = uint.Parse(data.Get("StatsUnspent"));

        Stats.Strength = uint.Parse(data.Get("StatsStrength"));
        Stats.Defense = uint.Parse(data.Get("StatsDefense"));
        Stats.Stamina = uint.Parse(data.Get("StatsStamina"));
        Stats.Energy = uint.Parse(data.Get("StatsEnergy"));

        CurrentLevel = uint.Parse(data.Get("CurrentLevel"));
        CurrentExp = uint.Parse(data.Get("CurrentExp"));
    }
}

[Serializable]
public class CharStats : System.Object
{
    public event EventHandler OnStatsChange;

    public uint PointCount { get { return strength + stamina + defense + energy; } }

    public CharStats()
    {
    }

    public CharStats(uint value)
    {
        strength = value;
        defense = value;
        stamina = value;
        energy = value;
    }

    public CharStats(uint str, uint def, uint stam, uint ener, EventHandler ev)
    {
        strength = str;
        defense = def;
        stamina = stam;
        energy = ener;
        OnStatsChange = ev;
    }

    [SerializeField]
    private uint strength = 1u;
    public uint Strength { get { return strength; }
        set
        {
            strength = value;
            if (OnStatsChange != null)
                OnStatsChange(this, new EventArgs());
        }
    }

    [SerializeField]
    private uint defense = 1u;
    public uint Defense { get { return defense; }
        set
        {
            defense = value;
            if (OnStatsChange != null)
                OnStatsChange(this, new EventArgs());
        }
    }

    [SerializeField]
    private uint stamina = 1u;
    public uint Stamina { get { return stamina; }
        set
        {
            stamina = value;
            if (OnStatsChange != null)
                OnStatsChange(this, new EventArgs());
        }
    }

    [SerializeField]
    private uint energy = 1u;
    public uint Energy { get { return energy; }
        set
        {
            energy = value;
            if (OnStatsChange != null)
                OnStatsChange(this, new EventArgs());
        }
    }

    public void FireEvent()
    {
        if (OnStatsChange != null)
            OnStatsChange(this, new EventArgs());
    }

    public static CharStats operator +(CharStats lhs, CharStats rhs)
    {
        return new CharStats(lhs.Strength + rhs.Strength,
            lhs.Defense + rhs.Defense,
            lhs.Stamina + rhs.Stamina,
            lhs.Energy + rhs.Energy,
            lhs.OnStatsChange + rhs.OnStatsChange);
    }
}

public enum StatType
{
    Stength,
    Defense,
    Stamina,
    Energy
}