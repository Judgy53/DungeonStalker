using UnityEngine;
using System.Collections;

public class MagicProjectileFireball : MagicProjectile
{

    [SerializeField]
    private float summonDistance = 1.0f;

    [SerializeField]
    private float speed = 10.0f;

    private void Start()
    {
        transform.position = launcher.transform.position + launcher.transform.forward * summonDistance;
        transform.rotation = launcher.transform.rotation;

        transform.localScale += transform.localScale * Power;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == launcher.gameObject)
            return;

        IDamageable target = collider.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            target.AddDamage(Damage);
            //target.AddEffect(EffectType.Burn, 10);
        }

        Destroy(gameObject);
    }
}
