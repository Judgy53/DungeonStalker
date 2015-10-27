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
            if (offHandWeapon != null)
                UnregisterCallbacks(offHandWeapon);

            if (OnOffHandWeaponChange != null)
                OnOffHandWeaponChange(this, new EventWeaponChange(offHandWeapon));
            offHandWeapon = value;

            if (offHandWeaponPoint != null)
            {
                GameObject go = (offHandWeapon as Behaviour).gameObject;
                go.transform.SetParent(offHandWeaponPoint);
                go.SetLayerRecursively(offHandWeaponPoint.gameObject.layer);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }

            RegisterCallbacks(offHandWeapon);
        }
    }

    private IWeapon mainHandWeapon = null;
    public IWeapon MainHandWeapon
    {
        get { return mainHandWeapon; }
        set
        {
            if (mainHandWeapon != null)
                UnregisterCallbacks(mainHandWeapon);

            if (OnMainHandWeaponChange != null)
                OnMainHandWeaponChange(this, new EventWeaponChange(mainHandWeapon));
            mainHandWeapon = value;

            if (mainHandWeaponPoint != null)
            {
                GameObject go = (mainHandWeapon as Behaviour).gameObject;
                go.transform.SetParent(mainHandWeaponPoint);
                go.SetLayerRecursively(mainHandWeaponPoint.gameObject.layer);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }

            RegisterCallbacks(mainHandWeapon);
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
        //You cannot adjust an animation speed with a parameter in blend trees, thanks Unity ...
        UpdateHitAnimatorSpeed("MainHand", mainHandWeapon);
        //UpdateHitAnimatorSpeed("OffHand", offHandWeapon);
    }

    private void OnPrimary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (weapSender == mainHandWeapon)
            SetAnimatorTrigger("MainHandPrimary");
        else
            SetAnimatorTrigger("OffHandPrimary");
    }

    private void OnEndPrimary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (weapSender == mainHandWeapon)
            SetAnimatorTrigger("MainHandEndPrimary");
        else
            SetAnimatorTrigger("OffHandEndPrimary");
    }

    private void OnSecondary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (weapSender == mainHandWeapon)
            SetAnimatorTrigger("MainHandSecondary");
        else
            SetAnimatorTrigger("OffHandSecondary");
    }

    private void OnEndSecondary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (weapSender == mainHandWeapon)
            SetAnimatorTrigger("MainHandEndSecondary");
        else
            SetAnimatorTrigger("OffHandEndSecondary");
    }

    private void RegisterCallbacks(IWeapon weapon)
    {
        weapon.OnPrimary += OnPrimary;
        weapon.OnEndPrimary += OnEndPrimary;
        weapon.OnSecondary += OnSecondary;
        weapon.OnEndSecondary += OnEndSecondary;
    }

    private void UnregisterCallbacks(IWeapon weapon)
    {
        weapon.OnPrimary -= OnPrimary;
        weapon.OnEndPrimary -= OnEndPrimary;
        weapon.OnSecondary -= OnSecondary;
        weapon.OnEndSecondary -= OnEndSecondary;
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

    private void UpdateHitAnimatorSpeed(string layerName, IWeapon weapon)
    {
        if (armsAnimator == null)
            return;

        AnimatorStateInfo stateInfo = armsAnimator.GetCurrentAnimatorStateInfo(armsAnimator.GetLayerIndex(layerName));

        if (stateInfo.IsName(layerName + ".Primary"))
            if (weapon is IPhysicalWeapon)
                armsAnimator.speed = 1.0f / (weapon as IPhysicalWeapon).AnimationTime;
            else
                armsAnimator.speed = 1.0f;
        else
            armsAnimator.speed = 1.0f;
    }

    /// <summary>
    /// Try to call Primary use for weapon (Will call main hand primary if not wearing offhand weapon)
    /// </summary>
    /// <param name="weapon">0 : main hand, 1 : off hand</param>
    public void Primary(int weapon)
    {
        if (offHandWeapon != null && weapon != 0)
            offHandWeapon.Primary();
        else if (mainHandWeapon != null)
            mainHandWeapon.Primary();
    }
    
    /// <summary>
    /// Try to call Primary use for weapon (Will call main hand primary if not wearing offhand weapon)
    /// </summary>
    /// <param name="weapon">0 : main hand, 1 : off hand</param>
    public void EndPrimary(int weapon)
    {
        if (offHandWeapon != null && weapon != 0)
            offHandWeapon.EndPrimary();
        else if (mainHandWeapon != null)
            mainHandWeapon.EndPrimary();
    }

    /// <summary>
    /// Try to call Secondary use for offhand weapon, if not wearing a offhand weapon, then it'll try to call the main hand one. (Will call main hand Primary if using two weapons)
    /// </summary>
    public void Secondary()
    {
        if (mainHandWeapon != null && offHandWeapon != null)
            Primary(0);

        if (offHandWeapon != null)
            offHandWeapon.Secondary();
        else if (mainHandWeapon != null)
            mainHandWeapon.Secondary();    
    }

    /// <summary>
    /// Try to call EndSecondary use for offhand weapon, if not wearing a offhand weapon, then it'll try to call the main hand one. (Will call main hand EndPrimary if using two weapons)
    /// </summary>
    public void EndSecondary()
    {
        if (mainHandWeapon != null && offHandWeapon != null)
            EndPrimary(0);

        if (offHandWeapon != null)
            offHandWeapon.EndSecondary();
        else if (mainHandWeapon != null)
            mainHandWeapon.EndSecondary();  
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
