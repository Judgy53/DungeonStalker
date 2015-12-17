using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalWeaponController : MonoBehaviour, IPhysicalWeapon, IBlockable
{
    public bool canUse = true;

    public event EventHandler<OnHitArgs> OnHit;

    public event EventHandler OnPrimary;
    public event EventHandler OnEndPrimary;
    public event EventHandler OnSecondary;
    public event EventHandler OnEndSecondary;

    public event EventHandler<OnKillArgs> OnKill;

    public GameObject hitEffectPrefab = null;

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
    private float minBlockValue = 0.2f;
    public float MinBlockValue { get { return minBlockValue; } set { minBlockValue = value; } }

    [SerializeField]
    private float maxBlockValue = 1.5f;
    public float MaxBlockValue { get { return maxBlockValue; } set { maxBlockValue = value; } }

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

    private PhysicalWeaponUseState useState = PhysicalWeaponUseState.Default;
    public PhysicalWeaponUseState UseState { get { return useState; } }

    [SerializeField]
    private CharStats gearStats = new CharStats(0);
    public CharStats GearStats { get { return gearStats; } }

    [SerializeField]
    private bool canFireContinuously = false;
    public bool CanFireContinuously { get { return canFireContinuously; } set { canFireContinuously = value; } }

    [SerializeField]
    private Vector3 handPositionOffset = Vector3.zero;
    public Vector3 HandPositionOffset { get { return handPositionOffset; } }

    [SerializeField]
    private Vector3 handRotationOffset = Vector3.zero;
    public Vector3 HandRotationOffset { get { return handRotationOffset; } }

    private float useTimer = 0.0f;

    private List<IDamageable> hits = new List<IDamageable>();

    private BlockEffect blockEffect = null;

    [SerializeField]
    private GameObject inventoryItemPrefab = null;
    public GameObject InventoryItemPrefab { get { return inventoryItemPrefab; } }

    private StatsManager stManager = null;

    [SerializeField]
    private bool autoFirePrimaryClip = false;
    public bool AutoFirePrimaryClip { get { return autoFirePrimaryClip; } }

    [SerializeField]
    private AudioClip[] attackClips;

    public void Primary()
    {
        if (!canUse) 
            return;

        if (useState == PhysicalWeaponUseState.Default)
        {
            if (OnPrimary != null)
                OnPrimary(this, new EventArgs());
            
            useState = PhysicalWeaponUseState.Attacking;
        }
    }

    public void EndPrimary()
    {
    }

    public void Secondary()
    {
        if (!canUse)
            return;

        if (useState == PhysicalWeaponUseState.Default)
        {
            if (OnSecondary != null)
                OnSecondary(this, new EventArgs());
            
            useState = PhysicalWeaponUseState.Blocking;

            EffectManager manager = GetComponentInParent<EffectManager>();
            if (manager != null)
            {
                blockEffect = new BlockEffect(minBlockValue, maxBlockValue);
                manager.AddEffect(blockEffect);
            }
        }
    }

    public void EndSecondary()
    {
        if (!canUse)
            return;

        if (useState == PhysicalWeaponUseState.Blocking)
        {
            if (OnEndSecondary != null)
                OnEndSecondary(this, new EventArgs());
            
            useState = PhysicalWeaponUseState.Default;

            EffectManager manager = GetComponentInParent<EffectManager>();
            if (manager != null && blockEffect != null)
            {
                manager.RemoveEffect(blockEffect);
                blockEffect = null;
            }
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
        if (useState == PhysicalWeaponUseState.Attacking)
        {
            useTimer += Time.fixedDeltaTime;

            Ray ray;
            RaycastHit[] hitInfos;
            float rootDistance;
            if (stManager != null)
            {
                rootDistance = Vector3.Distance(stManager.transform.position, startRaycast.position);
                ray = new Ray(stManager.transform.position, startRaycast.position - stManager.transform.position);
                hitInfos = Physics.RaycastAll(ray, rootDistance);
                
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * rootDistance, Color.cyan);

                foreach (var hit in hitInfos)
                {
                    if (!hit.collider.transform.IsChildOf(stManager.transform))
                    {
                        IDamageable damageable = null;
                        if ((damageable = hit.collider.GetComponentInParent<IDamageable>()) != null)
                            TryRegisterHit(hit, damageable);
                        else if (!hit.collider.isTrigger)
                        {
                            CheckReturnState();
                            return;
                        }
                    }
                }
            }

            ray = new Ray(startRaycast.position, endRaycast.position - startRaycast.position);
            hitInfos = Physics.RaycastAll(ray, raycastDistance);

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.cyan);

            foreach (var hit in hitInfos)
            {
                if (stManager == null || !hit.collider.transform.IsChildOf(stManager.transform))
                {
                    IDamageable damageable = null;
                    if ((damageable = hit.collider.GetComponentInParent<IDamageable>()) != null)
                        TryRegisterHit(hit, damageable);
                    else if (!hit.collider.isTrigger)
                    {
                        CheckReturnState();
                        return;
                    }
                }
            }

            CheckReturnState();
        }
    }

    private void CheckReturnState()
    {
        if (useTimer >= attackSpeed)
        {
            useTimer = 0.0f;
            useState = PhysicalWeaponUseState.Default;
            hits.Clear();

            if (OnEndPrimary != null)
                OnEndPrimary(this, new EventArgs());
        }
    }

    private void TryRegisterHit(RaycastHit hit, IDamageable damageable)
    {
        if (stManager == null)
        {
            if ((damageable as Behaviour).gameObject.tag == transform.root.tag)
                return;
        }
        else
        {
            if ((damageable as Behaviour).gameObject.tag == stManager.tag)
                return;
        }

        if ((damageable = hit.collider.GetComponentInParent<IDamageable>()) != null && (damageable as Behaviour).enabled)
        {
            if (!hits.Exists(x => x == damageable))
            {
                float damages = 0.0f;
                if (stManager != null)
                    damages = UnityEngine.Random.Range(minDamages, maxDamages) + (0.4f * stManager.Stats.Strength);
                else
                    damages = UnityEngine.Random.Range(minDamages, maxDamages);

                if (OnHit != null)
                    OnHit(this, new OnHitArgs(damageable, damages));

                if (stManager != null)
                {
                    if (damageable.WillKill(damages, stManager))
                    {
                        if (OnKill != null)
                            OnKill(this, new OnKillArgs(damageable));
                    }
                }
                else
                {
                    if (damageable.WillKill(damages))
                    {
                        if (OnKill != null)
                            OnKill(this, new OnKillArgs(damageable));
                    }
                }

                if (stManager != null)
                    damageable.AddDamage(damages, stManager);
                else
                    damageable.AddDamage(damages);

                hits.Add(damageable);

                if (hitEffectPrefab != null)
                    GameObject.Destroy(GameObject.Instantiate(hitEffectPrefab, hit.point, Quaternion.identity), 5.0f);
            }
        }
    }

    public void TransferToContainer(IContainer container)
    {
        if (inventoryItemPrefab != null)
            container.AddItem((GameObject.Instantiate(inventoryItemPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IItem>());

        GameObject.Destroy(this.gameObject);
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

    public void Save(SaveData data)
    {
        string path = ResourcesPathHelper.GetWeaponPath(this.WeaponType, this.name);

        data.Add("path", path);
    }

    public void Load(SaveData data)
    {
        //nothing to load
	}

    public string GetInventoryDescription()
    {
        string output = ""; 
        output += "\nDamages : " + minDamages + " - " + maxDamages;
        output += "\nAttack speed : " + attackSpeed;
        if (weaponHand == WeaponHand.OneHanded)
            output += "\nOneHanded";
        else
            output += "\nTwoHanded";

        return output;
    }


    public AudioClip GetPrimaryClip()
    {
        if (attackClips.Length == 0)
            return null;

        int tabPos = UnityEngine.Random.Range(0, attackClips.Length);
        AudioClip clip = attackClips[tabPos];

        return clip;
    }

    public AudioClip GetEndPrimaryClip()
    {
        return null;
    }
}
