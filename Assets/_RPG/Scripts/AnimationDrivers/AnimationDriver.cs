using UnityEngine;
using System.Collections;

public class AnimationDriver : AnimationDriverBase
{
    public string mainHandPrimaryName = "";
    public string mainHandEndPrimaryName = "";
    public string mainHandSecondaryName = "";
    public string mainHandEndSecondaryName = "";

    public string offHandPrimaryName = "";
    public string offHandEndPrimaryName = "";
    public string offHandSecondaryName = "";
    public string offHandEndSecondaryName = "";

    public string weaponRestrictionName = "";

    public string mainHandWeaponTypeName = "";
    public string offHandWeaponTypeName = "";

    public string mainHandWeaponSpeedLayerName = "";
    public string offHandWeaponSpeedLayerName = "";

    public string movementVelocityForwardName = "";
    public string movementVelocitySidesName = "";
    public string movementVelocityAirName = "";
    public string movementGroundedName = "";

    public string onHitName = "";
    public string onDeathName = "";

    protected override void OnAwake()
    {
        HealthManager hm = GetComponent<HealthManager>();
        if (hm != null)
        {
            hm.OnHit += OnHit;
            hm.OnDeath += OnDeath;
        }
    }

    public override void MainHandPrimary() 
    {
        if (mainHandPrimaryName != "")
            animator.SetTrigger(mainHandPrimaryName);
    }

    public override void MainHandEndPrimary() 
    {
        if (mainHandEndPrimaryName != "")
            animator.SetTrigger(mainHandEndPrimaryName);
    }

    public override void MainHandSecondary() 
    {
        if (mainHandSecondaryName != "")
            animator.SetTrigger(mainHandSecondaryName);
    }

    public override void MainHandEndSecondary() 
    {
        if (mainHandEndSecondaryName != "")
            animator.SetTrigger(mainHandEndSecondaryName);
    }

    public override void OffHandPrimary() 
    {
        if (offHandPrimaryName != "")
            animator.SetTrigger(offHandPrimaryName);
    }

    public override void OffHandEndPrimary() 
    {
        if (offHandEndPrimaryName != "")
            animator.SetTrigger(offHandEndPrimaryName);
    }

    public override void OffHandSecondary() 
    {
        if (offHandSecondaryName != "")
            animator.SetTrigger(offHandSecondaryName);
    }

    public override void OffHandEndSecondary() 
    {
        if (offHandEndSecondaryName != "")
            animator.SetTrigger(offHandEndSecondaryName);
    }

    public override void SetHandsRestrictions(IWeapon weapon) 
    {
        if (weaponRestrictionName != "")
            animator.SetFloat(weaponRestrictionName, (float)weapon.WeaponHand);
    }

    public override void SetWeaponType(IWeapon weapon, WeaponRestriction hand) 
    {
        switch (hand)
        {
            case WeaponRestriction.Both:
                if (mainHandWeaponTypeName != "")
                    animator.SetFloat(mainHandWeaponTypeName, (float)weapon.WeaponType);
                if (offHandWeaponTypeName != "")
                    animator.SetFloat(offHandWeaponTypeName, (float)weapon.WeaponType);
                break;
            case WeaponRestriction.MainHand:
                if (mainHandWeaponTypeName != "")
                    animator.SetFloat(mainHandWeaponTypeName, (float)weapon.WeaponType);
                break;
            case WeaponRestriction.OffHand:
                if (offHandWeaponTypeName != "")
                    animator.SetFloat(offHandWeaponTypeName, (float)weapon.WeaponType);
                break;
        }
    }

    public override void SetSpeed(IWeapon weapon, WeaponRestriction hand)
    {
        AnimatorStateInfo stateInfo;
        switch (hand)
        {
            case WeaponRestriction.Both:
                throw new System.InvalidOperationException();
            case WeaponRestriction.MainHand:
                if (mainHandWeaponSpeedLayerName != "")
                {
                    stateInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(mainHandWeaponSpeedLayerName));

                    if (stateInfo.IsName(mainHandWeaponSpeedLayerName + ".Primary"))
                    {
                        if (weapon is IPhysicalWeapon)
                            animator.speed = 1.0f / (weapon as IPhysicalWeapon).AnimationTime;
                        if (weapon is IRangedWeapon)
                            animator.speed = 1.0f / (weapon as IRangedWeapon).AnimationTime;
                    }
                    else
                        animator.speed = 1.0f;
                }
                break;
            case WeaponRestriction.OffHand:
                if (offHandWeaponSpeedLayerName != "")
                {
                    stateInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(offHandWeaponSpeedLayerName));

                    if (stateInfo.IsName(offHandWeaponSpeedLayerName + ".Primary"))
                    {
                        if (weapon is IPhysicalWeapon)
                            animator.speed = 1.0f / (weapon as IPhysicalWeapon).AnimationTime;
                        if (weapon is IRangedWeapon)
                            animator.speed = 1.0f / (weapon as IRangedWeapon).AnimationTime;
                    }
                    else
                        animator.speed = 1.0f;
                }
                break;
        }
    }

    public override void SetMovementVelocity(float forwardVelocity, float lateralVelocity, float airVelocity, bool grounded) 
    {
        if (movementVelocityForwardName != "")
            animator.SetFloat(movementVelocityForwardName, forwardVelocity);
        if (movementVelocitySidesName != "")
            animator.SetFloat(movementVelocitySidesName, lateralVelocity);
        if (movementVelocityAirName != "")
            animator.SetFloat(movementVelocityAirName, airVelocity);

        if (movementGroundedName != "")
            animator.SetBool(movementGroundedName, grounded);
    }

    public override void OnHit(object sender, System.EventArgs args)
    {
        if (onHitName != "")
            animator.SetTrigger(onHitName);
    }

    public override void OnDeath(object sender, System.EventArgs args)
    {
        if (onDeathName != "")
            animator.SetBool(onDeathName, true);
    }
}
