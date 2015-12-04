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

    private Vector3 direction = Vector3.forward;
    public Vector3 Direction { get { return direction; } set { direction = value.normalized; } }

    public void Initialize(IRangedWeapon weapon)
    {
        this.weapon = weapon;
        direction = transform.forward;
    }

    private void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == transform.tag)
            return;

        if (weapon != null)
        {
            IDamageable damageable = null;
            if ((damageable = other.GetComponentInParent<IDamageable>()) != null)
                Hit(damageable);
        }

        GameObject.Destroy(this.transform.root.gameObject);
    }

    public void Hit(IDamageable damageable)
    {
        float damages = UnityEngine.Random.Range(minDamages, maxDamages);
        if (damageable is HealthManager && (damageable as HealthManager).MaxHealth > damageable.Damage && damageable.WillKill(damages))
            weapon.ProjectileOnKillCallback(this, damageable, damages);
        else
            weapon.ProjectileHitCallback(this, damageable, damages);

        damageable.AddDamage(damages);
    }
}