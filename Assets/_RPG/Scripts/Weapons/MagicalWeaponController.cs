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
    private Vector3 handOffset = new Vector3(1.0f, 0.75f, 0.0f);
    public Vector3 HandOffset { get { return handOffset; } }

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
    private float chargeScaleFactor = 1.0f;
    public float ChargeScaleFactor { get { return chargeScaleFactor; } }

    [SerializeField]
    private GameObject projectilePrefab = null;
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    [SerializeField]
    private float cooldown = 1.0f;
    public float Cooldown { get { return cooldown; } set { cooldown = value; } }

    private float timeToWait = 1.0f;

    private Vector3 baseScale;

    private void Start()
    {
        timeToWait = cooldown;
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
            timeToWait = Mathf.Max(timeToWait - Time.fixedDeltaTime, 0f);
            if (timeToWait == 0f)
                canUse = true;
        }
        else if (useState == MagicalWeaponUseState.Charging)
        {
            currentChargeTime = Mathf.Min(currentChargeTime + Time.fixedDeltaTime, maxChargeTime);

            float scaleFactor = currentChargeTime / maxChargeTime;
            scaleFactor *= chargeScaleFactor;
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
            timeToWait = cooldown;
        }
    }

    private void LaunchProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Not Projectile bound to " + this.name);
            return;
        }

        GameObject projectile = Instantiate<GameObject>(projectilePrefab);

        MagicProjectile magic = projectile.GetComponent<MagicProjectile>();

        if (magic == null)
        {
            Debug.LogWarning("Projectile bound to " + this.name + " is not a magic projectile.");
            Destroy(projectile);
            return;
        }

        magic.Launcher = GetComponentInParent<WeaponManager>().gameObject;
        magic.Power = currentChargeTime / maxChargeTime;
    }
}
