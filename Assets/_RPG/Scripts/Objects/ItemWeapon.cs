using UnityEngine;
using System.Collections;

public class ItemWeapon : MonoBehaviour, IItem, IUsable
{
    [SerializeField]
    private Sprite image = null;
    public Sprite Image { get { return image; } }

    [SerializeField]
    private string objName = "DefaultWeapon";
    public string Name { get { return objName; } }

    [SerializeField]
    private string itemDescription = "Weapon";
    public string Description { get { return itemDescription; } }

    [SerializeField]
    private uint weigth = 15u;
    public uint Weigth { get { return weigth; } }

    [SerializeField]
    private bool canDrop = true;
    public bool CanDrop { get { return canDrop; } set { canDrop = value; } }

    [SerializeField]
    private GameObject dropPrefab = null;
    public GameObject DropPrefab { get { return dropPrefab; } }

    [SerializeField]
    private ItemType type = ItemType.Weapons;
    public ItemType Type { get { return type; } }

    [SerializeField]
    private ItemQuality quality = ItemQuality.Common;
    public ItemQuality Quality { get { return quality; } }

    public string actionName = "Equip";
    public string useDescription = "Equip this weapon";

    public GameObject weaponPrefab = null;

    private WeaponRestriction restriction = WeaponRestriction.Both;
    public WeaponRestriction Restriction { get { return restriction; } }

    public void Start()
    {
        restriction = weaponPrefab.GetComponent<IWeapon>().WeaponRestrictions;
    }

    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        return useDescription;
    }

    public void Use(InteractManager manager, UsableArgs args)
    {
        if (args != null && args is EquipWeaponArgs)
            Use(manager, args as EquipWeaponArgs);
        else
            Use(manager, new EquipWeaponArgs(EquipWeaponArgs.Hand.MainHand));
    }

    public void Use(InteractManager manager, EquipWeaponArgs args)
    {
        WeaponManager wpmanager = manager.gameObject.GetComponent<WeaponManager>();
        if (wpmanager != null)
        {
            if (args.hand == EquipWeaponArgs.Hand.MainHand)
                wpmanager.MainHandWeapon = (GameObject.Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IWeapon>();
            else
                wpmanager.OffHandWeapon = (GameObject.Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IWeapon>();
        }

        manager.gameObject.GetComponentInChildren<PlayerContainer>().RemoveItem(this);
        Destroy(this.gameObject);
    }

    public void OnPickup(IPickable pickable)
    {
    }

    public void OnDrop(IPickable pickable)
    {
    }
}

public class EquipWeaponArgs : UsableArgs
{
    public enum Hand 
    {
        MainHand,
        OffHand
    }

    public Hand hand = Hand.MainHand;

    public EquipWeaponArgs(Hand hand)
    {
        this.hand = hand;
    }
}
