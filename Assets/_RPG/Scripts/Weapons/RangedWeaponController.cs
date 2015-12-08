using System;
using System.Collections;
using UnityEngine;

public class RangedWeaponController : MonoBehaviour, IRangedWeapon
{
    public bool canUse = true;

    [SerializeField]
    private Transform fireMuzzle = null;
    public Transform FireMuzzle { get { return fireMuzzle; } }

    [SerializeField]
    private FloatMinMax baseDamages = new FloatMinMax(1.0f, 2.0f);
    public FloatMinMax BaseDamages { get { return baseDamages; } set { baseDamages = value; } }

    [SerializeField]
    private bool useAmmo = true;
    public bool UseAmmo { get { return useAmmo; } set { useAmmo = value; } }

    [SerializeField]
    private int consumedAmmoPerShot = 1;
    public int ConsumedAmmoPerShot { get { return consumedAmmoPerShot; } set { consumedAmmoPerShot = Mathf.Clamp(value, 0, int.MaxValue); } }

    [SerializeField]
    private float animationTime = 1.5f;
    public float AnimationTime { get { return animationTime; } set { animationTime = value; } }

    [SerializeField]
    private float projectileLifetime = 10.0f;
    public float ProjectileLifetime { get { return projectileLifetime; } set { projectileLifetime = value; } }

    [SerializeField]
    private float projectileDeviation = 0.0f;
    public float ProjectileDeviation { get { return projectileDeviation; } set { projectileDeviation = value; } }

    [SerializeField]
    private int projectilePerShot = 1;
    public int ProjectilePerShot { get { return projectilePerShot; } set { projectilePerShot = Mathf.Clamp(value, 0, int.MaxValue); } }

    public event EventHandler<OnHitArgs> OnHit;
    public event EventHandler<OnKillArgs> OnKill;

    public event EventHandler OnPrimary;
    public event EventHandler OnEndPrimary;
    public event EventHandler OnSecondary;
    public event EventHandler OnEndSecondary;

    [SerializeField]
    private Vector3 handPositionOffset = Vector3.zero;
    public Vector3 HandPositionOffset { get { return handPositionOffset; } }

    [SerializeField]
    private Vector3 handRotationOffset = Vector3.zero;
    public Vector3 HandRotationOffset { get { return handRotationOffset; } }

    [SerializeField]
    private CharStats gearStats = new CharStats(0);
    public CharStats GearStats { get { return gearStats; } }

    [SerializeField]
    private WeaponHand weaponHand = WeaponHand.TwoHanded;
    public WeaponHand WeaponHand { get { return weaponHand; } }

    [SerializeField]
    private WeaponRestriction weaponRestrictions = WeaponRestriction.MainHand;
    public WeaponRestriction WeaponRestrictions { get { return weaponRestrictions; } }

    [SerializeField]
    private WeaponType weaponType = WeaponType.Gun;
    public WeaponType WeaponType { get { return weaponType; } }

    [SerializeField]
    private GameObject inventoryItemPrefab = null;
    public GameObject InventoryItemPrefab { get { return inventoryItemPrefab; } }

    [SerializeField]
    private bool canFireContinuously = false;
    public bool CanFireContinuously { get { return canFireContinuously; } set { canFireContinuously = value; } }

    private RangedWeaponState state = RangedWeaponState.Idle;
    public RangedWeaponState State { get { return state; } }

    public IRangedWeaponAmmo Ammo 
    { 
        get 
        {
            if (weaponManager != null)
                return weaponManager.CurrentAmmos;
            return null;
        }

        set
        {
            if (weaponManager != null)
                weaponManager.CurrentAmmos = value;
        }
    }

    private StatsManager stManager = null;
    private WeaponManager weaponManager = null;

    private void Start()
    {
        if (fireMuzzle == null)
        {
            Debug.LogError(this.name + " isn't configured properly.");
            enabled = false;
            return;
        }
    }

    public virtual void Primary()
    {
        if (!canUse)
            return;

        if (state == RangedWeaponState.Idle)
        {
            if (Ammo != null && Ammo.AmmoLeft != 0)
            {
                if (OnPrimary != null)
                    OnPrimary(this, new EventArgs());

                state = RangedWeaponState.Firing;
                StartCoroutine(FiringBehavior());

                Ammo.UseAmmo(consumedAmmoPerShot);
            }
            else
                Debug.Log("Out of ammo !");
        }
    }

    public virtual void EndPrimary()
    {
    }

    public virtual void Secondary()
    {
    }

    public virtual void EndSecondary()
    {
    }

    public void OnEquip(WeaponManager manager)
    {
        stManager = GetComponentInParent<StatsManager>();
        stManager.GearStats += gearStats;
        weaponManager = manager;
    }

    public void OnUnequip()
    {
        stManager.GearStats -= gearStats;
        stManager = null; 
        weaponManager = null;
    }

    public void TransferToContainer(IContainer container)
    {
        if (inventoryItemPrefab != null)
            container.AddItem((GameObject.Instantiate(inventoryItemPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IItem>());

        GameObject.Destroy(this.gameObject);
    }

    private IEnumerator FiringBehavior()
    {
        float halfDeviation = projectileDeviation / 2.0f;
        RaycastHit hit;
        Vector3 worldhp = Camera.main.GetWorldHitpoint(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f), out hit);
        bool isAngleValid = Vector3.Angle(fireMuzzle.forward, worldhp - fireMuzzle.position) < 45.0f;
        IDamageable dmg = null;
        if (Vector3.Angle(fireMuzzle.forward, worldhp - fireMuzzle.position) > 45.0f)
            dmg = hit.collider.gameObject.GetComponentInParent<IDamageable>();


        for (int i = 0; i < projectilePerShot; i++)
        {
            if (!isAngleValid && dmg == null)
                break;

            IRangedWeaponProjectile p = Ammo.InstantiateProjectile(fireMuzzle);
            p.Initialize(this);

            Ammo.ApplyEffect(p);

            GameObject.Destroy((p as Behaviour).gameObject, projectileLifetime);

            if (!isAngleValid)
            {
                p.Hit(dmg);
                continue;
            }

            p.Direction = (worldhp - (p as Behaviour).transform.position).normalized;
            p.Direction = Quaternion.Euler(UnityEngine.Random.Range(-halfDeviation, halfDeviation),
                    UnityEngine.Random.Range(-halfDeviation, halfDeviation),
                    UnityEngine.Random.Range(-halfDeviation, halfDeviation)) * p.Direction;
        }
        
        yield return new WaitForSeconds(animationTime);

        if (OnEndPrimary != null)
            OnEndPrimary(this, new EventArgs());

        state = RangedWeaponState.Idle;
    }

    public void ProjectileHitCallback(IRangedWeaponProjectile p, IDamageable target, float damages)
    {
        if (OnHit != null)
            OnHit(this, new OnHitArgs(target, damages));
    }

    public void ProjectileOnKillCallback(IRangedWeaponProjectile p, IDamageable target, float damages)
    {
        if (OnKill != null)
            OnKill(this, new OnKillArgs(target));
    }

    public void Save(SaveData data)
    {
        string path = ResourcesPathHelper.GetWeaponPath(this.WeaponType, this.name);

        data.Add("path", path);
    }

    public void Load(SaveData data)
    {
        //nothing to load
	}
	
    public string GetInventoryDescription()
    {
        string output = "";
        output += "\nBase damages : " + baseDamages.min + " - " + baseDamages.max;
        output += "\nFire rate : " + animationTime;
        output += "\nConsumed ammo per shot : " + consumedAmmoPerShot;
        output += "\nProjectiles per shot : " + projectilePerShot;
        output += "\nProjectile deviation : " + projectileDeviation;

        return output;
    }
}
