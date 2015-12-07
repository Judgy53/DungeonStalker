using UnityEngine;
using System.Collections;

public class ArmorManager : MonoBehaviour
{
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

    private void RecomputeGearStats()
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
        }
    }
}

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
    public const int Waist = 5;
    public const int Legs = 6;
    public const int Feets = 7;
    public const int Ring = 8;
    public const int Trinket = 9;

    public const int Count = 10;

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

    private enum ArmorSlotHelper
    {
        Head = 0,
        Neck = 1,
        Shoulders = 2,
        Chest = 3,
        Back = 4,
        Waist = 5,
        Legs = 6,
        Feets = 7,
        Ring = 8,
        Trinket = 9
    }
}

public enum ArmorType
{
    Plate,
    Mail,
    Leather,
    Cloth
}

public class Armor : ScriptableObject
{
    [SerializeField]
    private string armorName = "";
    public string Name { get { return armorName; } }

    [SerializeField]
    private ArmorSlot slot = ArmorSlot.Head;
    public ArmorSlot Slot { get { return slot; } }

    [SerializeField]
    private ArmorType type = ArmorType.Cloth;
    public ArmorType Type { get { return type; } }

    [SerializeField]
    private int armor = 100;
    public int ArmorValue { get { return armor; } }

    [SerializeField]
    private CharStats stats = new CharStats(0);
    public CharStats Stats { get { return stats; } }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("0.0f : broken, 1.0f : new.")]
    private float state = 1.0f;
    public float State { get { return state; } set { state = value; } }

    [SerializeField]
    private GameObject itemPrefab = null;
    public GameObject ItemPrefab { get { return itemPrefab; } }

    public void TransferToContainer(IContainer container)
    {
        if (container == null)
            return;

        if (itemPrefab != null)
            container.AddItem((GameObject.Instantiate(itemPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IItem>());

        ScriptableObject.Destroy(this);
    }
}
