using UnityEngine;
using System.Collections;

#region ARMOR
[System.Serializable]
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
#endregion
