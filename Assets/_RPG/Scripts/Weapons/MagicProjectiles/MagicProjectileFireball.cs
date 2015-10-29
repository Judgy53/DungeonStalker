﻿using UnityEngine;
using System.Collections;

public class MagicProjectileFireball : MagicProjectile
{
    [SerializeField]
    private float summonDistance = 1.0f;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(1f, 1f, 1f);

    private void Start()
    {
        transform.position = launcher.transform.position + launcher.transform.forward * summonDistance;
        transform.rotation = launcher.transform.rotation;

        speed -= speed / 2f * Power;

        Vector3 scale = transform.localScale;
        scale.Scale(scaleFactor * (Power + 1f));
        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == launcher.gameObject || collider.gameObject.layer == LayerMask.NameToLayer("FirstPass"))
            return;

        IDamageable target = collider.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            target.AddDamage(Damage);
        }

        Destroy(gameObject);
    }
}
