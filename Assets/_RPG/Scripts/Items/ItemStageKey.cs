using UnityEngine;
using System.Collections;

public class ItemStageKey : MonoBehaviour, IItem
{
    [SerializeField]
    private Sprite image = null;
    public Sprite Image { get { return image; } }

    public string Name
    {
        get
        {
            return "Stage " + stageValue + " key";
        }
    }

    [SerializeField]
    private string itemDescription = "You'd better keep that around ...";
    public string Description { get { return itemDescription; } }

    [SerializeField]
    private uint weigth = 0u;
    public uint Weigth { get { return weigth; } }

    private bool canDrop = false;
    public bool CanDrop { get { return canDrop; } set { canDrop = value; } }

    public GameObject DropPrefab { get { return null; } }

    [SerializeField]
    private ItemType type = ItemType.Misc;
    public ItemType Type { get { return type; } }

    [SerializeField]
    private ItemQuality quality = ItemQuality.Legendary;
    public ItemQuality Quality { get { return quality; } }

    [SerializeField]
    private int stageValue = 1;
    public int StageValue { get { return stageValue; } }

    public void OnPickup(IPickable pickable)
    {
        if (pickable.UserData != null)
            stageValue = (int)pickable.UserData;
        else
            stageValue = (int)GameManager.Stage;
    }

    public void OnDrop(IPickable pickable)
    {
        pickable.UserData = stageValue;
    }

    public void Save(SaveData data)
    {
        data.Add("stageValue", stageValue);
    }

    public void Load(SaveData data)
    {
        stageValue = int.Parse(data.Get("stageValue"));
    }

    public void Initialize(int stage)
    {
        stageValue = stage;
    }
}
