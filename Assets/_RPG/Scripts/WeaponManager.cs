using UnityEngine;
using System.Collections;
using System;

public class WeaponManager : MonoBehaviour, ISavable
{
    public event EventHandler<EventWeaponChange> OnMainHandWeaponChange;
    public event EventHandler<EventWeaponChange> OnOffHandWeaponChange;

    public event EventHandler<OnKillArgs> OnKill;
    public event EventHandler<OnHitArgs> OnHit;

    public GameObject debugMainHandStartWeaponPrefab = null;
    public GameObject debugOffHandStartWeaponPrefab = null;

    public Transform mainHandWeaponPoint = null;
    public Transform offHandWeaponPoint = null;

    private IWeapon offHandWeapon = null;
    public IWeapon OffHandWeapon { get { return offHandWeapon; }
        set 
        {
            if (offHandWeapon != null)
            {
                offHandWeapon.OnUnequip();
                UnregisterCallbacks(offHandWeapon);
                IContainer container = GetComponentInChildren<IContainer>();
                if (container != null)
                    offHandWeapon.TransferToContainer(container);
            }

            if (OnOffHandWeaponChange != null)
                OnOffHandWeaponChange(this, new EventWeaponChange(offHandWeapon));
            offHandWeapon = value;

            if (offHandWeaponPoint != null)
            {
                GameObject go = (offHandWeapon as Behaviour).gameObject;
                go.transform.SetParent(offHandWeaponPoint);
                if (tag == "Player")
                    go.SetLayerRecursively(offHandWeaponPoint.gameObject.layer);
                else
                    go.SetLayerRecursively(LayerMask.NameToLayer("Default"));
                go.transform.localPosition = new Vector3(-offHandWeapon.HandPositionOffset.x, offHandWeapon.HandPositionOffset.y, offHandWeapon.HandPositionOffset.z);
                go.transform.localRotation = Quaternion.Euler(offHandWeapon.HandRotationOffset);
            }

            RegisterCallbacks(offHandWeapon);

            value.OnEquip(this);
        }
    }

    private IWeapon mainHandWeapon = null;
    public IWeapon MainHandWeapon
    {
        get { return mainHandWeapon; }
        set
        {
            if (mainHandWeapon != null)
            {
                mainHandWeapon.OnUnequip();
                UnregisterCallbacks(mainHandWeapon);
                IContainer container = GetComponentInChildren<IContainer>();
                if (container != null)
                    mainHandWeapon.TransferToContainer(container);
            }

            if (OnMainHandWeaponChange != null)
                OnMainHandWeaponChange(this, new EventWeaponChange(mainHandWeapon));
            mainHandWeapon = value;

            if (mainHandWeaponPoint != null)
            {
                GameObject go = (mainHandWeapon as Behaviour).gameObject;
                go.transform.SetParent(mainHandWeaponPoint);
                if (tag == "Player")
                    go.SetLayerRecursively(mainHandWeaponPoint.gameObject.layer);
                else
                    go.SetLayerRecursively(LayerMask.NameToLayer("Default"));
                go.transform.localPosition = mainHandWeapon.HandPositionOffset;
                go.transform.localRotation = Quaternion.Euler(mainHandWeapon.HandRotationOffset);
            }

            RegisterCallbacks(mainHandWeapon);

            value.OnEquip(this);
        }
    }

    private AnimationDriverBase driver = null;

    private bool mhPrimaryEnabled = false;
    private bool ohPrimaryEnabled = false;

    private void Start()
    {
        driver = GetComponent<AnimationDriverBase>();

        if (debugMainHandStartWeaponPrefab != null)
            MainHandWeapon = (GameObject.Instantiate(debugMainHandStartWeaponPrefab, Vector3.zero, debugMainHandStartWeaponPrefab.transform.rotation) as GameObject).GetComponent<IWeapon>();

        if (debugOffHandStartWeaponPrefab != null)
            OffHandWeapon = (GameObject.Instantiate(debugOffHandStartWeaponPrefab, Vector3.zero, debugOffHandStartWeaponPrefab.transform.rotation) as GameObject).GetComponent<IWeapon>();
    }

    private void Update()
    {
        if (mainHandWeapon != null)
        {
            if (driver != null)
            {
                driver.SetHandsRestrictions(mainHandWeapon);
                driver.SetSpeed(mainHandWeapon, WeaponRestriction.MainHand);
                driver.SetWeaponType(mainHandWeapon, WeaponRestriction.MainHand);
            }

            if (mainHandWeapon.CanFireContinuously && mhPrimaryEnabled)
                Primary(0);
        }
        if (offHandWeapon != null)
        {
            if (driver != null)
            {
                driver.SetWeaponType(offHandWeapon, WeaponRestriction.OffHand);
                driver.SetSpeed(offHandWeapon, WeaponRestriction.OffHand);
            }

            if (offHandWeapon.CanFireContinuously && ohPrimaryEnabled)
                Primary(1);
        }
    }

    private void OnPrimary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (driver != null)
        {
            if (weapSender == mainHandWeapon)
            {
                if (weapSender.WeaponHand == WeaponHand.TwoHanded)
                    driver.OffHandPrimary();
                driver.MainHandPrimary();
            }
            else
                driver.OffHandPrimary();
        }
    }

    private void OnEndPrimary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (driver != null)
        {
            if (weapSender == mainHandWeapon)
            {
                if (weapSender.WeaponHand == WeaponHand.TwoHanded)
                    driver.OffHandEndPrimary();
                driver.MainHandEndPrimary();
            }
            else
                driver.OffHandEndPrimary();
        }
    }

    private void OnSecondary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (driver != null)
        {
            if (weapSender == mainHandWeapon)
            {
                if (weapSender.WeaponHand == WeaponHand.TwoHanded)
                    driver.OffHandSecondary();
                driver.MainHandSecondary();
            }
            else
                driver.OffHandSecondary();
        }
    }

    private void OnEndSecondary(object sender, EventArgs args)
    {
        IWeapon weapSender = sender as IWeapon;
        if (driver != null)
        {
            if (weapSender == mainHandWeapon)
            {
                if (weapSender.WeaponHand == WeaponHand.TwoHanded)
                    driver.OffHandEndSecondary();
                driver.MainHandEndSecondary();
            }
            else
                driver.OffHandEndSecondary();
        }
    }

    private void OnKillCallback(object sender, OnKillArgs args)
    {
        if (OnKill != null)
            OnKill(sender, args);
    }

    public void OnHitCallback(object sender, OnHitArgs args)
    {
        if (OnHit != null)
            OnHit(sender, args);
    }

    private void RegisterCallbacks(IWeapon weapon)
    {
        weapon.OnPrimary += OnPrimary;
        weapon.OnEndPrimary += OnEndPrimary;
        weapon.OnSecondary += OnSecondary;
        weapon.OnEndSecondary += OnEndSecondary;
        weapon.OnKill += OnKillCallback;
        weapon.OnHit += OnHitCallback;
    }

    private void UnregisterCallbacks(IWeapon weapon)
    {
        weapon.OnPrimary -= OnPrimary;
        weapon.OnEndPrimary -= OnEndPrimary;
        weapon.OnSecondary -= OnSecondary;
        weapon.OnEndSecondary -= OnEndSecondary;
        weapon.OnKill -= OnKillCallback;
        weapon.OnHit -= OnHitCallback;
    }

    /// <summary>
    /// Try to call Primary use for weapon (Will call main hand primary if not wearing offhand weapon)
    /// </summary>
    /// <param name="weapon">0 : main hand, 1 : off hand</param>
    public void Primary(int weapon)
    {
        if (offHandWeapon != null && weapon != 0)
        {
            offHandWeapon.Primary();
            ohPrimaryEnabled = true;
        }
        else if (mainHandWeapon != null)
        {
            mainHandWeapon.Primary();
            mhPrimaryEnabled = true;
        }
    }
    
    /// <summary>
    /// Try to call Primary use for weapon (Will call main hand primary if not wearing offhand weapon)
    /// </summary>
    /// <param name="weapon">0 : main hand, 1 : off hand</param>
    public void EndPrimary(int weapon)
    {
        if (offHandWeapon != null && weapon != 0)
        {
            offHandWeapon.EndPrimary();
            ohPrimaryEnabled = false;
        }
        else if (mainHandWeapon != null)
        {
            mainHandWeapon.EndPrimary();
            mhPrimaryEnabled = false;
        }
    }

    /// <summary>
    /// Try to call Secondary use for offhand weapon, if not wearing a offhand weapon, then it'll try to call the main hand one. (Will call main hand Primary if using two weapons)
    /// </summary>
    public void Secondary()
    {
        if (mainHandWeapon != null && offHandWeapon != null)
        {
            Primary(0);
            return;
        }

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
        {
            EndPrimary(0);
            return;
        }

        if (offHandWeapon != null)
            offHandWeapon.EndSecondary();
        else if (mainHandWeapon != null)
            mainHandWeapon.EndSecondary();  
    }

    public void Save(SaveData data)
    {
        if (MainHandWeapon != null)
            MainHandWeapon.ToSaveData(data, "MainHandWeapon");

        if(OffHandWeapon != null)
            OffHandWeapon.ToSaveData(data, "OffHandWeapon");
    }

    public void Load(SaveData data)
    {
        if (MainHandWeapon != null)
        {
            Destroy((MainHandWeapon as Behaviour).gameObject);
            mainHandWeapon = null; // set the variable (not the property) to null to avoid transfering to inventory
        }

        if (OffHandWeapon != null)
        {
            Destroy((OffHandWeapon as Behaviour).gameObject);
            offHandWeapon = null; // set the variable (not the property) to null to avoid transfering to inventory
        }


        string mainHandPath = data.Get("MainHandWeapon");
        string offHandPath = data.Get("OffHandWeapon");

        if(mainHandPath != null)
        {
            GameObject prefab = Resources.Load<GameObject>(mainHandPath);
            if (prefab == null)
                Debug.LogWarning("Loading Weapon MainHand : Failed to load \"" + mainHandPath + "\"");
            else
            {
                GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                
                MainHandWeapon = instance.GetComponent<IWeapon>();
            }
        }

        if (offHandPath != null)
        {
            GameObject prefab = Resources.Load<GameObject>(offHandPath);
            if (prefab == null)
                Debug.LogWarning("Loading Weapon OffHand : Failed to load \"" + offHandPath + "\"");
            else
            {
                GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

                OffHandWeapon = instance.GetComponent<IWeapon>();
            }
        }
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
