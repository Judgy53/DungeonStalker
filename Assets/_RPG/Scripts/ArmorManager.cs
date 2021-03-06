﻿using UnityEngine;
using System.Collections;
using System;

#region ARMOR_MGR
public class ArmorManager : MonoBehaviour
{
    public event EventHandler OnArmorChanged;

    [SerializeField]
    [Tooltip("Head 0, Neck 1, Shoulders 2, Chest 3, Back 4, Waist 5, Legs 6, Feets 7, Ring 8, Trinket 9")]
    private Armor[] debugStartArmor = new Armor[ArmorSlot.Count];

    [SerializeField]
    private int baseArmor = 0;
    public int BaseArmor { get { return baseArmor; } }

    public Armor Head { get { return armor[ArmorSlot.Head]; } 
        set
        {
            if (armor[ArmorSlot.Head] != null)
                armor[ArmorSlot.Head].TransferToContainer(container);
            armor[ArmorSlot.Head] = value;
            RecomputeGearStats();
        }
    }
    public Armor Neck { get { return armor[ArmorSlot.Neck]; } 
        set
        {
            if (armor[ArmorSlot.Neck] != null)
                armor[ArmorSlot.Neck].TransferToContainer(container);
            armor[ArmorSlot.Neck] = value;
            RecomputeGearStats(); 
        }
    }
    public Armor Shoulders { get { return armor[ArmorSlot.Shoulders]; } 
        set
        {
            if (armor[ArmorSlot.Shoulders] != null)
                armor[ArmorSlot.Shoulders].TransferToContainer(container);
            armor[ArmorSlot.Shoulders] = value; 
            RecomputeGearStats();
        } 
    }
    public Armor Chest { get { return armor[ArmorSlot.Chest]; } 
        set 
        {
            if (armor[ArmorSlot.Chest] != null)
                armor[ArmorSlot.Chest].TransferToContainer(container);
            armor[ArmorSlot.Chest] = value;
            RecomputeGearStats();
        }
    }
    public Armor Back { get { return armor[ArmorSlot.Back]; } 
        set
        {
            if (armor[ArmorSlot.Back] != null)
                armor[ArmorSlot.Back].TransferToContainer(container);
            armor[ArmorSlot.Back] = value; 
            RecomputeGearStats(); 
        }
    }
    public Armor Wirst
    {
        get { return armor[ArmorSlot.Wirst]; }
        set
        {
            if (armor[ArmorSlot.Wirst] != null)
                armor[ArmorSlot.Wirst].TransferToContainer(container);
            armor[ArmorSlot.Wirst] = value;
            RecomputeGearStats();
        }
    }
    public Armor Hands
    {
        get { return armor[ArmorSlot.Hands]; }
        set
        {
            if (armor[ArmorSlot.Hands] != null)
                armor[ArmorSlot.Hands].TransferToContainer(container);
            armor[ArmorSlot.Hands] = value;
            RecomputeGearStats();
        }
    }
    public Armor Waist { get { return armor[ArmorSlot.Waist]; } 
        set
        {
            if (armor[ArmorSlot.Waist] != null)
                armor[ArmorSlot.Waist].TransferToContainer(container);
            armor[ArmorSlot.Waist] = value;
            RecomputeGearStats(); 
        }
    }
    public Armor Legs { get { return armor[ArmorSlot.Legs]; } 
        set
        {
            if (armor[ArmorSlot.Legs] != null)
                armor[ArmorSlot.Legs].TransferToContainer(container);
            armor[ArmorSlot.Legs] = value; 
            RecomputeGearStats(); 
        }
    }
    public Armor Feets { get { return armor[ArmorSlot.Feets]; }
        set
        {
            if (armor[ArmorSlot.Feets] != null)
                armor[ArmorSlot.Feets].TransferToContainer(container);
            armor[ArmorSlot.Feets] = value; 
            RecomputeGearStats();
        }
    }
    public Armor Ring { get { return armor[ArmorSlot.Ring]; } 
        set 
        {
            if (armor[ArmorSlot.Ring] != null)
                armor[ArmorSlot.Ring].TransferToContainer(container);
            armor[ArmorSlot.Ring] = value; 
            RecomputeGearStats();
        }
    }
    public Armor Trinket { get { return armor[ArmorSlot.Trinket]; } 
        set
        {
            if (armor[ArmorSlot.Trinket] != null)
                armor[ArmorSlot.Trinket].TransferToContainer(container);
            armor[ArmorSlot.Trinket] = value; 
            RecomputeGearStats();
        }
    }

    private Armor[] armor = new Armor[ArmorSlot.Count];
    public Armor[] Armor { get { return armor; } }

    private CharStats totalGearStats = new CharStats(0);
    private StatsManager stmanager = null;
    private IContainer container = null;
    
    public int TotalArmor 
    {
        get 
        {
            int ret = baseArmor;
            foreach (Armor a in armor)
            {
                if (a != null)
                    ret += a.ArmorValue;
            }

            return ret;
        }
    }

    private void Start()
    {
        stmanager = GetComponent<StatsManager>();
        if (stmanager == null)
            Debug.LogWarning("No StatsManager found for " + this.name + " (" + gameObject.name + ")");

        container = GetComponentInChildren<IContainer>();

        for (int i = 0; i < ArmorSlot.Count; i++)
        {
            if (debugStartArmor[i] != null)
                armor[i] = ScriptableObject.Instantiate(debugStartArmor[i]) as Armor;
        }

        RecomputeGearStats();
    }

    public void RecomputeGearStats()
    {
        if (stmanager != null)
        {
            stmanager.GearStats -= totalGearStats;

            totalGearStats = new CharStats(0);
            foreach (Armor a in armor)
            {
                if (a != null)
                    totalGearStats += a.Stats;
            }

            stmanager.GearStats += totalGearStats;
            if (OnArmorChanged != null)
                OnArmorChanged(this, new EventArgs());
        }
    }
}
#endregion

#region ARMOR_SLOT
[System.Serializable]
public struct ArmorSlot
{
    [SerializeField]
    private ArmorSlotHelper InternalValue;

    public const int Head = 0;
    public const int Neck = 1;
    public const int Shoulders = 2;
    public const int Chest = 3;
    public const int Back = 4;
    public const int Wirst = 5;
    public const int Hands = 6;
    public const int Waist = 7;
    public const int Legs = 8;
    public const int Feets = 9;
    public const int Ring = 10;
    public const int Trinket = 11;

    public const int Count = 12;

    public static implicit operator ArmorSlot(int other)
    {
        return new ArmorSlot
        {
            InternalValue = (ArmorSlotHelper)other
        };
    }

    public static implicit operator int(ArmorSlot other)
    {
        return (int)other.InternalValue;
    }

    public enum ArmorSlotHelper
    {
        Head = 0,
        Neck = 1,
        Shoulders = 2,
        Chest = 3,
        Back = 4,
        Wirst = 5,
        Hands = 6,
        Waist = 7,
        Legs = 8,
        Feets = 9,
        Ring = 10,
        Trinket = 11
    }
}
#endregion

public enum ArmorType
{
    Plate,
    Mail,
    Leather,
    Cloth,
    Misc
}
