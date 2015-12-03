using UnityEngine;
using System.Collections;

public class StandardProjectile : MonoBehaviour, IRangedWeaponProjectile
{
    private IRangedWeapon weapon = null;
    public IRangedWeapon Weapon { get { return weapon; } }

    [SerializeField]
    private float minDamages = 5.0f;
    public float MinDamages { get { return minDamages; } set { minDamages = value; } }

    [SerializeField]
    private float maxDamages = 10.0f;
    public float MaxDamages { get { return maxDamages; } set { maxDamages = value; } }

    public float speed = 5.0f;

    public void Initialize(IRangedWeapon weapon)
    {
        this.weapon = weapon;
    }

    private void FixedUpdate()
    {
        transform.Translate(transform.forward * speed * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (weapon != null)
        {
            IDamageable damageable = null;
            if ((damageable = other.GetComponentInParent<IDamageable>()) != null)
            {
                float damages = UnityEngine.Random.Range(minDamages, maxDamages);
                if (damageable.WillKill(damages))
                    weapon.ProjectileOnKillCallback(this, damageable, damages);
                else
                    weapon.ProjectileHitCallback(this, damageable, damages);
            }
        }

        GameObject.Destroy(this.gameObject);
    }
}