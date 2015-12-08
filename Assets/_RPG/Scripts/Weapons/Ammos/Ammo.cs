using UnityEngine;
using System.Collections;

public class Ammo : ScriptableObject, IRangedWeaponAmmo
{
    public event System.EventHandler OnOutOfAmmo;

    [SerializeField]
    private int ammoLeft = 100;
    public int AmmoLeft { get { return ammoLeft; } set { ammoLeft = value; } }

    [SerializeField]
    private GameObject projectilePrefab = null;
    private IRangedWeaponProjectile projectile = null;
    public IRangedWeaponProjectile Projectile
    {
        get 
        {
            if (projectile == null && projectilePrefab != null)
                projectile = projectilePrefab.GetComponent<IRangedWeaponProjectile>();

            return projectile;
        }
    }

    [SerializeField]
    private FloatMinMax addedDamages = new FloatMinMax(1.0f, 2.0f);
    public FloatMinMax AddedDamages { get { return addedDamages; } set { addedDamages = value; } }

    [SerializeField]
    private GameObject itemGo = null;
    private IItem itemPrefab = null;
    public IItem ItemPrefab 
    {
        get 
        {
            if (itemPrefab == null && itemGo != null)
                itemPrefab = itemGo.GetComponent<IItem>();

            return itemPrefab;
        } 
    }

    public IRangedWeaponProjectile InstantiateProjectile(Transform transform)
    {
        return GameObject.Instantiate(Projectile as Behaviour, transform.position, transform.rotation) as IRangedWeaponProjectile;
    }

    public void ApplyEffect(IRangedWeaponProjectile projectile)
    {
        projectile.MinDamages += addedDamages.min;
        projectile.MaxDamages += addedDamages.max;
    }

    public bool UseAmmo(int ammoConsumed)
    {
        ammoLeft -= ammoConsumed;

        Debug.Log("Ammo left : " + ammoLeft);

        if (ammoLeft <= 0)
        {
            if (OnOutOfAmmo != null)
                OnOutOfAmmo(this, new System.EventArgs());
            ScriptableObject.Destroy(this);
            return false;
        }

        return ammoLeft > 0;
    }

    public void TransferToContainer(IContainer container)
    {
        if (ItemPrefab != null)
        {
            IItem item = GameObject.Instantiate(ItemPrefab as Behaviour, Vector3.zero, Quaternion.identity) as IItem;
            if (item is ItemAmmo)
                (item as ItemAmmo).ammoLeft = ammoLeft;

            container.AddItem(item);
        }

        ScriptableObject.Destroy(this);
    }

    public void Save(SaveData data)
    {
        data.Add("type", GetType().ToString());
        data.Add("left", ammoLeft);

        //save projectile prefab
        if(projectilePrefab != null)
            data.Add("projectilePath", ResourcesPathHelper.GetProjectilePath(projectilePrefab.name));
    }

    public void Load(SaveData data)
    {
        ammoLeft = int.Parse(data.Get("left"));

        //Load projectile prefab
        string projectilePrefabPath = data.Get("projectilePath");

        if (projectilePrefabPath != null)
            projectilePrefab = Resources.Load<GameObject>(projectilePrefabPath);
    }
}
