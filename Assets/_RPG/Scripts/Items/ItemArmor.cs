using UnityEngine;
using System.Collections;

public class ItemArmor : MonoBehaviour, IItem, IUsable
{
    [SerializeField]
    private Sprite image = null;
    public Sprite Image { get { return image; } }

    [SerializeField]
    private string itemName = "ArmorName";
    public string Name { get { return itemName; } }

    public string actionName = "Equip";
    public string useDescription = "Equip Armor.";

    [SerializeField]
    private string description = "Armor description";
    public string Description { get { return description; } }

    [SerializeField]
    private uint weight = 20u;
    public uint Weigth { get { return weight; } }

    [SerializeField]
    private ItemType type = ItemType.Armor;
    public ItemType Type { get { return type; } }

    [SerializeField]
    private ItemQuality quality = ItemQuality.Common;
    public ItemQuality Quality { get { return quality; } }

    [SerializeField]
    private bool canDrop = true;
    public bool CanDrop { get { return canDrop; } set { canDrop = value; } }

    [SerializeField]
    private GameObject dropPrefab = null;
    public GameObject DropPrefab { get { return dropPrefab; } }

    [SerializeField]
    private ScriptableObject armorPrefab = null;
    
    public void OnPickup(IPickable pickable)
    {
    }

    public void OnDrop(IPickable pickable)
    {
    }

    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        return useDescription;
    }

    public void Use(InteractManager user, UsableArgs args = null)
    {
        Armor a = ScriptableObject.Instantiate(armorPrefab as ScriptableObject) as Armor;
        ArmorManager am = user.GetComponentInChildren<ArmorManager>();
        switch (a.Slot)
        {
            case ArmorSlot.Head:
                am.Head = a;
                break;
            case ArmorSlot.Neck:
                am.Neck = a;
                break;
            case ArmorSlot.Shoulders:
                am.Shoulders = a;
                break;
            case ArmorSlot.Chest:
                am.Chest = a;
                break;
            case ArmorSlot.Back:
                am.Back = a;
                break;
            case ArmorSlot.Waist:
                am.Waist = a;
                break;
            case ArmorSlot.Legs:
                am.Legs = a;
                break;
            case ArmorSlot.Feets:
                am.Feets = a;
                break;
            case ArmorSlot.Ring:
                am.Ring = a;
                break;
            case ArmorSlot.Trinket:
                am.Trinket = a;
                break;
        }

        user.gameObject.GetComponentInChildren<IContainer>().RemoveItem(this);
        Destroy(this.gameObject);
    }

    public void Save(SaveData data)
    {
        // nothing to save
    }

    public void Load(SaveData data)
    {
        // nothing to load
    }
}
