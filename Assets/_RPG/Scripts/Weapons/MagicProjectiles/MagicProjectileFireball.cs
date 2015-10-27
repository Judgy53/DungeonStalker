using UnityEngine;
using System.Collections;

public class MagicProjectileFireball : MagicProjectile
{

    [SerializeField]
    private float summonDistance = 2.0f;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private float minDamage = 2.0f;

    [SerializeField]
    private float maxDamage = 10.0f;

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
        if (collider.gameObject == launcher)
            return;

        IDamageable target = collider.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            target.AddDamage(minDamage + ((maxDamage - minDamage) * Power));
            //target.AddEffect(EffectType.Burn, 10);
        }

        Destroy(gameObject);
    }
}
