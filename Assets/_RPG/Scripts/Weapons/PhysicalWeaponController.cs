using UnityEngine;
using System.Collections;
using System;

public class PhysicalWeaponController : MonoBehaviour, IPhysicalWeapon
{
    public bool canUse = true;

    public event EventHandler OnHit;

    public event EventHandler OnStartAttack;
    public event EventHandler OnBlock;

    [SerializeField]
    private float damages = 1.0f;
    public float Damages { get { return damages; } set { damages = value; } }

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

    public void Primary()
    {
        if (!canUse) 
            return;

        if (useState == WeaponUseState.Default)
            useState = WeaponUseState.Attacking;
    }

    public void EndPrimary()
    {
    }

    public void Secondary()
    {
        throw new NotImplementedException();
    }

    public void EndSecondary()
    {
        throw new NotImplementedException();
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

#pragma warning disable 0168
            foreach (var hit in hitInfos)
            {
                throw new NotImplementedException();
            }
#pragma warning restore 0168

            if (useTimer >= attackSpeed)
            {
                useTimer = 0.0f;
                useState = WeaponUseState.Default;
            }
        }
    }
}
