using System;
using UnityEngine;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    [SerializeField]
    private CharStats stats = new CharStats();
    public CharStats Stats { get { return stats; } }

    [SerializeField]
    private bool debugFireEvent = false;

    //Fire this script's Start after every other Starts.
    private void Start()
    {
        stats.FireEvent();
    }

    private void Update()
    {
        if (debugFireEvent)
        {
            stats.FireEvent();
            debugFireEvent = false;
        }
    }
}

[Serializable]
public class CharStats : System.Object
{
    public event EventHandler OnStatsChange;

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
}
