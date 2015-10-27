using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PhysicalWeaponController : MonoBehaviour, IPhysicalWeapon
{
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
    private float attackSpeed = 1.0f;
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }

    [SerializeField]
    private float animationTime = 1.0f;
    public float AnimationTime { get { return animationTime; } set { animationTime = value; } }

    [SerializeField]
    private Transform startRaycast = null;
    public Transform StartRaycast { get { return startRaycast; } }

    [SerializeField]
    private Transform endRaycast = null;
    public Transform EndRaycast { get { return endRaycast; } }

    private float raycastDistance = 0.0f;

    [SerializeField]
    private WeaponHand weaponHand = WeaponHand.OneHanded;
    public WeaponHand WeaponHand { get { return weaponHand;} }

    [SerializeField]
    private WeaponRestriction weaponRestriction = WeaponRestriction.Both;
    public WeaponRestriction WeaponRestrictions { get { return weaponRestriction; } }

    [SerializeField]
    private WeaponType weaponType = WeaponType.Sword;
    public WeaponType WeaponType { get { return weaponType; } }

    private WeaponUseState useState = WeaponUseState.Default;
    public WeaponUseState UseState { get { return useState; } }

    private float useTimer = 0.0f;

    private List<IDamageable> hits = new List<IDamageable>();

    public void Primary()
    {
        if (!canUse) 
            return;

        if (useState == WeaponUseState.Default)
        {
            if (OnPrimary != null)
                OnPrimary(this, new EventArgs());
            useState = WeaponUseState.Attacking;
        }
    }

    public void EndPrimary()
    {
    }

    public void Secondary()
    {
        if (!canUse)
            return;

        if (useState == WeaponUseState.Default)
        {
            if (OnSecondary != null)
                OnSecondary(this, new EventArgs());
            useState = WeaponUseState.Blocking;
        }
    }

    public void EndSecondary()
    {
        if (!canUse)
            return;

        if (useState == WeaponUseState.Blocking)
        {
            if (OnEndSecondary != null)
                OnEndSecondary(this, new EventArgs());
            useState = WeaponUseState.Default;
        }
    }

    private void Start()
    {
        if (startRaycast == null || endRaycast == null)
        {
            Debug.LogWarning(this.name + " is not configured properly ! Disabled script.");
            enabled = false;
            return;
        }

        raycastDistance = Vector3.Distance(startRaycast.position, endRaycast.position);
    }

    private void FixedUpdate()
    {
        if (useState == WeaponUseState.Attacking)
        {
            useTimer += Time.fixedDeltaTime;

            Ray ray = new Ray(startRaycast.position, endRaycast.position - startRaycast.position);
            RaycastHit[] hitInfos = Physics.RaycastAll(ray, raycastDistance);

            foreach (var hit in hitInfos)
            {
                IDamageable damageable = null;
                if ((damageable = hit.collider.GetComponentInParent<IDamageable>()) != null)
                {
                    if (!hits.Exists(x => x == damageable))
                    {
                        damageable.AddDamage(UnityEngine.Random.Range(minDamages, maxDamages));
                        hits.Add(damageable);
                    }
                }
            }

            if (useTimer >= attackSpeed)
            {
                useTimer = 0.0f;
                useState = WeaponUseState.Default;
                hits.Clear();
            }
        }
    }
}
