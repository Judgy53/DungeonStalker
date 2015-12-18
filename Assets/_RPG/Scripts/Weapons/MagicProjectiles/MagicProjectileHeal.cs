using UnityEngine;
using System.Collections;

public class MagicProjectileHeal : MagicProjectile
{
    [SerializeField]
    private float summonDistance = 0.0f;

    [SerializeField]
    private float speed = 2.0f;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(1f, 1f, 1f);

    private bool setuped = false;

    private void Start()
    {
        transform.position = launcher.transform.position + launcher.transform.forward * summonDistance;

        transform.rotation = launcher.transform.rotation;

        Vector3 scale = transform.localScale;
        scale.Scale(scaleFactor * (Power + 1f));
        transform.localScale = scale;

        setuped = true;

        Destroy(gameObject, 2.0f);

        HealthManager health = launcher.GetComponent<HealthManager>();

        if (health != null)
            health.Heal(damage);
    }

    private void FixedUpdate()
    {
        if(setuped)
            transform.position += transform.up * speed * Time.fixedDeltaTime;
    }
}
