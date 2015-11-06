using UnityEngine;
using System.Collections;
using System;

public class WeaponManager : MonoBehaviour, ISavable
{
    public event EventHandler<EventWeaponChange> OnMainHandWeaponChange;
    public event EventHandler<EventWeaponChange> OnOffHandWeaponChange;

    public event EventHandler<OnKillArgs> OnKill;

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
                // HACK
                offHandWeaponPoint.DetachChildren();
                go.transform.localScale = transform.lossyScale;
                go.transform.parent = offHandWeaponPoint;
            }

            RegisterCallbacks(offHandWeapon);

            value.OnEquip();
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
                // HACK
                mainHandWeaponPoint.DetachChildren();
                go.transform.localScale = transform.lossyScale;
                go.transform.parent = mainHandWeaponPoint;
            }

            RegisterCallbacks(mainHandWeapon);

            value.OnEquip();
        }
    }

    public Animator armsAnimator = null;

    private void Start()
    {
        if (armsAnimator == null)
            Debug.LogWarning(this.name + " : No arms animator specified.");

        if (debugMainHandStartWeaponPrefab != null)
            MainHandWeapon = (GameObject.Instantiate(debugMainHandStartWeaponPrefab, Vector3.zero, debugMainHandStartWeaponPrefab.transform.rotation) as GameObject).GetComponent<IWeapon>();

        if (debugOffHandStartWeaponPrefab != null)
            OffHandWeapon = (GameObject.Instantiate(debugOffHandStartWeaponPrefab, Vector3.zero, debugOffHandStartWeaponPrefab.transform.rotation) as GameObject).GetComponent<IWeapon>();
    }

    private void Update()
    {
        //You cannot adjust an animation speed with a parameter in blend trees, thanks Unity ...
        
        if (mainHandWeapon != null)
        {
            SetAnimatorHandsRestriction(mainHandWeapon, "HandRestriction");
            UpdateHitAnimatorSpeed("MainHand", mainHandWeapon);
            SetAnimatorWeaponType(mainHandWeapon, "MainHandWeaponType");
        }
        if (offHandWeapon != null)
        {
            SetAnimatorWeaponType(offHandWeapon, "OffHandWeaponType");
            UpdateHitAnimatorSpeed("OffHand", offHandWeapon);
        }
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

    private void OnKillCallback(object sender, OnKillArgs args)
    {
        if (OnKill != null)
            OnKill(sender, args);
    }

    private void RegisterCallbacks(IWeapon weapon)
    {
        weapon.OnPrimary += OnPrimary;
        weapon.OnEndPrimary += OnEndPrimary;
        weapon.OnSecondary += OnSecondary;
        weapon.OnEndSecondary += OnEndSecondary;
        weapon.OnKill += OnKillCallback;
    }

    private void UnregisterCallbacks(IWeapon weapon)
    {
        weapon.OnPrimary -= OnPrimary;
        weapon.OnEndPrimary -= OnEndPrimary;
        weapon.OnSecondary -= OnSecondary;
        weapon.OnEndSecondary -= OnEndSecondary;
        weapon.OnKill -= OnKillCallback;
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
