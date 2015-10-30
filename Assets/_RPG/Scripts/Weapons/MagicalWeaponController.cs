using UnityEngine;
using System.Collections;
using System;

public class MagicalWeaponController : MonoBehaviour, IMagicalWeapon
{
    [HideInInspector]
    public bool canUse = true;

    public event EventHandler OnHit;

    public event EventHandler OnPrimary;
    public event EventHandler OnEndPrimary;
    public event EventHandler OnSecondary;
    public event EventHandler OnEndSecondary;

    [SerializeField]
    private float minDamages = 1.0f;
    public float MinDamages { get { return minDamages; } set { minDamages = value; } }

    [SerializeField]
    private float maxDamages = 1.0f;
    public float MaxDamages { get { return maxDamages; } set { maxDamages = value; } }

    [SerializeField]
    private float maxChargeTime = 3.0f;
    public float MaxChargeTime { get { return maxChargeTime; } set { maxChargeTime = value; } }

    private float currentChargeTime = 0.0f;
    public float CurrentChargeTime { get { return currentChargeTime; } }

    [SerializeField]
    private WeaponHand weaponHand = WeaponHand.OneHanded;
    public WeaponHand WeaponHand { get { return weaponHand; } }

    [SerializeField]
    private WeaponRestriction weaponRestriction = WeaponRestriction.Both;
    public WeaponRestriction WeaponRestrictions { get { return weaponRestriction; } }

    [SerializeField]
    private WeaponType weaponType = WeaponType.Magic;
    public WeaponType WeaponType { get { return weaponType; } }

    private MagicalWeaponUseState useState = MagicalWeaponUseState.Default;
    public MagicalWeaponUseState UseState { get { return useState; } }

    [SerializeField]
    private Vector3 handPositionOffset = Vector3.zero;
    public Vector3 HandPositionOffset { get { return handPositionOffset; } }

    [SerializeField]
    private Vector3 handRotationOffset = Vector3.zero;
    public Vector3 HandRotationOffset { get { return handRotationOffset; } }

    [SerializeField]
    private float sizeChargeScale = 1.0f;
    public float SizeChargeScale { get { return sizeChargeScale; } }

    [SerializeField]
    private GameObject projectilePrefab = null;
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    [SerializeField]
    private float cooldown = 1.0f;
    public float Cooldown { get { return cooldown; } set { cooldown = value; } }

    private float currentCD = 1.0f;

    [SerializeField]
    private float manaCost = 10.0f;
    public float ManaCost { get { return manaCost; } set { manaCost = value; } }

    [SerializeField]
    private float manaChargeScale = 1.0f;
    public float ManaChargeScale { get { return manaChargeScale; } set { manaChargeScale = value; } }

    public float MaxManaCost { get { return manaCost * (1 + ManaChargeScale); } }

    private Vector3 baseScale;

    [SerializeField]
    private GameObject inventoryItemPrefab = null;
    public GameObject InventoryItemPrefab { get { return inventoryItemPrefab; } }

    private void Start()
    {
        currentCD = cooldown;
    }

    public void Primary()
    {
        if (!canUse)
            return;

        if (useState == MagicalWeaponUseState.Default)
        {
            if (OnPrimary != null)
                OnPrimary(this, new EventArgs());
            useState = MagicalWeaponUseState.Charging;
            baseScale = transform.localScale;
        }
    }

    public void EndPrimary()
    {
        if (!canUse)
            return;

        if (useState == MagicalWeaponUseState.Charging)
        {
            if (OnPrimary != null)
                OnEndPrimary(this, new EventArgs());
            useState = MagicalWeaponUseState.Launching;
        }
    }

    public void Secondary()
    {
    }

    public void EndSecondary()
    {

    }

    private void FixedUpdate()
    {
        if(useState == MagicalWeaponUseState.Default)
        {
            currentCD = Mathf.Max(currentCD - Time.fixedDeltaTime, 0f);
            if (currentCD == 0f)
                canUse = true;
        }
        else if (useState == MagicalWeaponUseState.Charging)
        {
            currentChargeTime = Mathf.Min(currentChargeTime + Time.fixedDeltaTime, maxChargeTime);

            float scaleFactor = currentChargeTime / maxChargeTime;
            scaleFactor *= sizeChargeScale;
            scaleFactor += 1f;

            transform.localScale = baseScale * scaleFactor;

        }
        else if (useState == MagicalWeaponUseState.Launching)
        {
            LaunchProjectile();

            currentChargeTime = 0f;
            transform.localScale = baseScale;
            useState = MagicalWeaponUseState.Default;

            canUse = false;
            currentCD = cooldown;
        }
    }

    private void LaunchProjectile()
    {
        ManaManager launcher = GetComponentInParent<ManaManager>();

        float chargeNormalized = currentChargeTime / maxChargeTime;

        float realManaCost = manaCost * (1 + ManaChargeScale * chargeNormalized);

        if (CanLaunch(launcher, realManaCost))
        {
            GameObject projectile = Instantiate<GameObject>(projectilePrefab);

            MagicProjectile magic = projectile.GetComponent<MagicProjectile>();

            magic.Launcher = launcher;
            magic.Power = chargeNormalized;
            magic.Damage = minDamages + ((maxDamages - minDamages) * chargeNormalized);

            launcher.RemoveMana(realManaCost);
        }
    }

    private bool CanLaunch(ManaManager launcher, float realManaCost)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Not Projectile bound to " + this.name);
            return false;
        }

        if(projectilePrefab.GetComponent<MagicProjectile>() == null)
        {
            Debug.LogWarning("Projectile bound to " + this.name + " is not a magic projectile.");
            return false;
        }

        if (launcher == null)
        {
            WeaponManager weaponLauncher = GetComponentInParent<WeaponManager>();
            Debug.LogWarning(this.name + "launched by " + weaponLauncher.name + " but do not use mana.");
            return false;
        }

        if (launcher.CurrentMana < realManaCost)
            return false;

        return true;
    }

    public void TransferToContainer(IContainer container)
    {
        if (inventoryItemPrefab != null)
            container.AddItem((GameObject.Instantiate(inventoryItemPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IItem>());

        GameObject.Destroy(this.gameObject);
    }
}
