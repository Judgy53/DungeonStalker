using UnityEngine;
using System.Collections;
using System;

public class WeaponManager : MonoBehaviour
{
    public event EventHandler<EventWeaponChange> OnMainHandWeaponChange;
    public event EventHandler<EventWeaponChange> OnOffHandWeaponChange;

    public GameObject debugMainHandStartWeapon = null;
    public GameObject debugOffHandStartWeapon = null;

    public Transform mainHandWeaponPoint = null;
    public Transform offHandWeaponPoint = null;

    private IWeapon offHandWeapon = null;
    public IWeapon OffHandWeapon { get { return offHandWeapon; }
        set 
        {
            if (OnOffHandWeaponChange != null)
                OnOffHandWeaponChange(this, new EventWeaponChange(offHandWeapon));
            offHandWeapon = value;

            if (offHandWeaponPoint != null)
            {
                GameObject go = (offHandWeapon as Behaviour).gameObject;
                go.transform.SetParent(offHandWeaponPoint);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private IWeapon mainHandWeapon = null;
    public IWeapon MainHandWeapon
    {
        get { return mainHandWeapon; }
        set
        {
            if (OnMainHandWeaponChange != null)
                OnMainHandWeaponChange(this, new EventWeaponChange(mainHandWeapon));
            mainHandWeapon = value;

            if (mainHandWeaponPoint != null)
            {
                GameObject go = (mainHandWeapon as Behaviour).gameObject;
                go.transform.SetParent(mainHandWeaponPoint);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }
        }
    }

    public Animator armsAnimator = null;

    private void Start()
    {
        if (armsAnimator == null)
            Debug.LogWarning(this.name + " : No arms animator specified.");

        if (debugMainHandStartWeapon != null)
            MainHandWeapon = debugMainHandStartWeapon.GetComponent<IWeapon>();

        if (debugOffHandStartWeapon != null)
            OffHandWeapon = debugOffHandStartWeapon.GetComponent<IWeapon>();
    }

    private void Update()
    {
        if (mainHandWeapon == null && offHandWeapon == null)
            return;

        if (mainHandWeapon != null && offHandWeapon == null)
        {
            SetAnimatorTimeModifier(mainHandWeapon, "MainHandTimeModifier");
            SetAnimatorHandsRestriction(mainHandWeapon, "HandRestriction");
            SetAnimatorWeaponType(mainHandWeapon, "MainHandWeaponType");

            if (Input.GetButtonDown("Fire1"))
            {
                mainHandWeapon.Primary();
                SetAnimatorTrigger("MainHandPrimary");
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                mainHandWeapon.EndPrimary();
                SetAnimatorTrigger("MainHandEndPrimary");
            }
            if (Input.GetButtonDown("Fire2"))
            {
                mainHandWeapon.Secondary();
                SetAnimatorTrigger("MainHandSecondary");
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                mainHandWeapon.EndSecondary();
                SetAnimatorTrigger("MainHandEndSecondary");
            }
        }
        else if (mainHandWeapon != null && offHandWeapon != null)
        {
            SetAnimatorTimeModifier(mainHandWeapon, "MainHandTimeModifier");
            SetAnimatorHandsRestriction(mainHandWeapon, "HandRestriction");
            SetAnimatorWeaponType(mainHandWeapon, "MainHandWeaponType");

            SetAnimatorTimeModifier(offHandWeapon, "OffHandTimeModifier");
            //SetAnimatorHandsRestriction(offHandWeapon, "OffHandRestriction");
            SetAnimatorWeaponType(offHandWeapon, "OffHandWeaponType");

            if (Input.GetButtonDown("Fire1"))
            {
                mainHandWeapon.Primary();
                SetAnimatorTrigger("MainHandPrimary");
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                mainHandWeapon.EndPrimary();
                SetAnimatorTrigger("MainHandEndPrimary");
            }
            if (Input.GetButtonDown("Fire2"))
            {
                offHandWeapon.Primary();
                SetAnimatorTrigger("OffHandPrimary");
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                offHandWeapon.EndPrimary();
                SetAnimatorTrigger("OffHandEndPrimary");
            }
        }
        else if (mainHandWeapon == null && offHandWeapon != null)
        {
            SetAnimatorTimeModifier(offHandWeapon, "OffHandTimeModifier");
            SetAnimatorHandsRestriction(offHandWeapon, "HandRestriction");
            SetAnimatorWeaponType(offHandWeapon, "OffHandWeaponType");

            if (Input.GetButtonDown("Fire1"))
            {
                offHandWeapon.Primary();
                SetAnimatorTrigger("OffHandPrimary");
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                offHandWeapon.EndPrimary();
                SetAnimatorTrigger("OffHandEndPrimary");
            }
            if (Input.GetButtonDown("Fire2"))
            {
                offHandWeapon.Secondary();
                SetAnimatorTrigger("OffHandSecondary");
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                offHandWeapon.EndSecondary();
                SetAnimatorTrigger("OffHandEndSecondary");
            }
        }
    }

    private void SetAnimatorTimeModifier(IWeapon weapon, string parameterName)
    {
        if (weapon is IPhysicalWeapon)
            if (armsAnimator != null)
                armsAnimator.SetFloat(parameterName, 1.0f / (weapon as IPhysicalWeapon).AnimationTime);
    }

    private void SetAnimatorHandsRestriction(IWeapon weapon, string parameterName)
    {
        if (armsAnimator != null)
            armsAnimator.SetFloat(parameterName, (float)weapon.WeaponHand);
    }

    private void SetAnimatorWeaponType(IWeapon weapon, string parameterName)
    {
        if (armsAnimator != null)
            armsAnimator.SetFloat(parameterName, (float)weapon.WeaponType);
    }

    private void SetAnimatorTrigger(string parameterName)
    {
        if (armsAnimator != null)
            armsAnimator.SetTrigger(parameterName);
    }
}

public class EventWeaponChange : EventArgs
{
    public IWeapon newWeapon;

    public EventWeaponChange(IWeapon weap)
    {
        newWeapon = weap;
    }
}
