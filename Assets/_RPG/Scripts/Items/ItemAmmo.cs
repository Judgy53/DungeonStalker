using UnityEngine;
using System.Collections;

public class ItemAmmo : MonoBehaviour, IItem, IUsable
{
    [SerializeField]
    private Sprite image = null;
    public Sprite Image { get { return image; } }

    [SerializeField]
    private string itemName = "Ammo";
    public string Name { get { return itemName; } }

    public string actionName = "Equip";
    public string useDescription = "Use ammunitions.";

    [SerializeField]
    private string description = "Standard ammunitions";
    public string Description 
    { 
        get 
        {
            string outd = "";
            outd += description + "\nAdds " + Ammo.AddedDamages.min + " to " + Ammo.AddedDamages.max + " damages to a ranged weapon.";
            outd += "\nAmmo left : " + (ammoLeft >= 0 ? ammoLeft : Ammo.AmmoLeft);

            return outd;
        }
    }

    [SerializeField]
    private uint weight = 20u;
    public uint Weigth { get { return weight; } }

    [SerializeField]
    private ItemType type = ItemType.Ammo;
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
    private ScriptableObject ammoPrefab = null;
    private IRangedWeaponAmmo ammo = null;
    public IRangedWeaponAmmo Ammo 
    { 
        get
        {
            if (ammo == null && ammoPrefab != null)
                ammo = ammoPrefab as IRangedWeaponAmmo;

            return ammo;
        }
    }

    public int ammoLeft = -1;

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
        IRangedWeaponAmmo a = ScriptableObject.Instantiate(ammo as ScriptableObject) as IRangedWeaponAmmo;
        WeaponManager wp = user.GetComponentInChildren<WeaponManager>();
        wp.CurrentAmmos = a;

        if (ammoLeft >= 0)
            a.AmmoLeft = ammoLeft;
        
        user.gameObject.GetComponentInChildren<IContainer>().RemoveItem(this);
        Destroy(this.gameObject);
    }

    public void OnPickup(IPickable pickable)
    {
        if (pickable != null && pickable.UserData != null)
            ammoLeft = (int)pickable.UserData;
    }

    public void OnDrop(IPickable pickable)
    {
        if (pickable != null)
            pickable.UserData = (object)ammoLeft;
    }

    public void Save(SaveData data)
    {
        data.Add("ammoCount", ammoLeft);
    }

    public void Load(SaveData data)
    {
        ammoLeft = int.Parse(data.Get("ammoCount"));
    }
}
