using UnityEngine;
using System.Collections;

public class MagicProjectileFireball : MagicProjectile
{
    [SerializeField]
    private float summonDistance = 1.0f;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(1f, 1f, 1f);

    [SerializeField]
    private AudioClip loopClip;

    [SerializeField]
    private AudioClip hitClip;

    private bool setuped = false;

    private void Start()
    {
        transform.position = launcher.transform.position + launcher.transform.forward * summonDistance;
        transform.Translate(0f, 0.2f, 0f);

        transform.rotation = launcher.transform.rotation;

        speed -= speed / 3f * Power;

        Vector3 scale = transform.localScale;
        scale.Scale(scaleFactor * (Power + 1f));
        transform.localScale = scale;

        AudioManager.PlaySfx(loopClip, transform, 1.0f, true);

        setuped = true;
    }

    private void FixedUpdate()
    {
        if(setuped)
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!setuped)
            return;

        if (collider.transform.IsChildOf(launcher.transform)|| collider.gameObject.layer == LayerMask.NameToLayer("FirstPass"))
            return; // Don't collide with launcher or player arms

        if (collider.gameObject.GetComponent<MagicProjectile>())
            return; // Don't collide with other projectiles

        IDamageable target = collider.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            if (target.WillKill(Damage))
                if (OnKill != null)
                    OnKill(target);
                
            target.AddDamage(Damage);
        }

        AudioManager.PlaySfx(hitClip, transform.position);

        Destroy(gameObject);
    }
}
