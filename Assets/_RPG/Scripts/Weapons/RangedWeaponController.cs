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
    private GameObject projectilePrefab = null;

    private IRangedWeaponProjectile projectile = null;
    public IRangedWeaponProjectile Projectile { get { return projectile; } }

    private IRangedWeaponAmmo ammo = null;
    public IRangedWeaponAmmo Ammo { get { return ammo; } set { ammo = value; } }

    [SerializeField]
    private bool useAmmo = true;
    public bool UseAmmo { get { return useAmmo; } set { useAmmo = value; } }

    [SerializeField]
    private float animationTime = 1.5f;
    public float AnimationTime { get { return animationTime; } set { animationTime = value; } }

    [SerializeField]
    private float projectileLifetime = 10.0f;
    public float ProjectileLifetime { get { return projectileLifetime; } set { projectileLifetime = value; } }

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

    private RangedWeaponState state = RangedWeaponState.Idle;
    public RangedWeaponState State { get { return state; } }

    [SerializeField]
    private bool canFireContinuously = false;
    public bool CanFireContinuously { get { return canFireContinuously; } set { canFireContinuously = value; } }

    private StatsManager stManager = null;

    private void Start()
    {
        if (fireMuzzle == null || projectilePrefab == null)
        {
            Debug.LogError(this.name + " isn't configured properly.");
            enabled = false;
            return;
        }

        projectile = projectilePrefab.GetComponent<IRangedWeaponProjectile>();
    }
    
    public virtual void Primary()
    {
        if (!canUse)
            return;

        if (state == RangedWeaponState.Idle)
        {
            /*if (ammo != null && ammo.AmmoLeft != 0)
            {*/
                if (OnPrimary != null)
                {
                    OnPrimary(this, new EventArgs());
                    /*if (OnEndPrimary != null)
                        OnEndPrimary(this, new EventArgs());*/
                }

                state = RangedWeaponState.Firing;
                StartCoroutine(FiringBehavior());

                /*if (!ammo.UseAmmo())
                    ammo = null;*/
            //}
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
    }

    public void OnUnequip()
    {
        stManager.GearStats -= gearStats;
        stManager = null;
    }

    public void TransferToContainer(IContainer container)
    {
    }

    public void ToSaveData(SaveData data, string name)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator FiringBehavior()
    {
        IRangedWeaponProjectile p = GameObject.Instantiate(projectile as Behaviour, fireMuzzle.position, fireMuzzle.rotation) as IRangedWeaponProjectile;
        p.Initialize(this);
        GameObject.Destroy(p as Behaviour, projectileLifetime);

        //ammo.ApplyEffect(p);

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
}
