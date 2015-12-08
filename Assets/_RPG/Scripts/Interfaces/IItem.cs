using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IItem : ISavable
{
    Sprite Image { get; }
    string Name { get; }
    string Description { get; }
    uint Weigth { get; }
    ItemType Type { get; }
    ItemQuality Quality { get; }

    bool CanDrop { get; set; }
    GameObject DropPrefab { get; }

    void OnPickup(IPickable pickable);
    void OnDrop(IPickable pickable);
}

public enum ItemType
{
    Misc = 0,
    Weapons,
    Food,
    Ammo,
    Armor
}

public enum ItemQuality
{
    Junk = 0,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public static class ItemQualityExpansions
{
    static readonly Color[] colors = 
    {
        Color.gray,
        Color.white,
        Color.green,
        Color.blue,
        new Color(153.0f / 255.0f, 0.0f, 153.0f / 255.0f),
        new Color(1.0f, 128.0f / 255.0f, 0.0f)
    };

    public static Color ToColor(this ItemQuality q)
    {
        return colors[(int)q];
    }
}
