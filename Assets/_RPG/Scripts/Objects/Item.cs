using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour, IItem
{
    [SerializeField]
    private Sprite image = null;
    public Sprite Image { get { return image; } }

    [SerializeField]
    private string objName = "A really heavy boulder";
    public string Name { get { return objName; } }

    [SerializeField]
    private string itemDescription = "Why would you carry that ?";
    public string Description { get { return itemDescription; } }

    [SerializeField]
    private uint weigth = 150u;
    public uint Weigth { get { return weigth; } }

    [SerializeField]
    private bool canDrop = true;
    public bool CanDrop { get { return canDrop; } set { canDrop = value; } }

    [SerializeField]
    private GameObject dropPrefab = null;
    public GameObject DropPrefab { get { return dropPrefab; } }

    [SerializeField]
    private ItemType type = ItemType.Misc;
    public ItemType Type { get { return type; } }

    [SerializeField]
    private ItemQuality quality = ItemQuality.Common;
    public ItemQuality Quality { get { return quality; } }

    public void OnPickup(IPickable pickable)
    {
    }

    public void OnDrop(IPickable pickable)
    {
    }
}
