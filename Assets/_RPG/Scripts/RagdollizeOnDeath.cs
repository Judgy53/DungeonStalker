using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class RagdollizeOnDeath : MonoBehaviour
{
    public float delay = 1.0f;
    private Animator animator = null;

    private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    private void Start()
    {
        GetComponent<HealthManager>().OnDeath += RagdollizeOnDeath_OnDeath;
        animator = GetComponent<Animator>();
        Rigidbody parentRigidbody = GetComponent<Rigidbody>();

        rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        rigidbodies.Remove(parentRigidbody);

        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = true;
    }

    private void RagdollizeOnDeath_OnDeath(object sender, System.EventArgs e)
    {
        StartCoroutine(RagdollizeBehavior());
    }

    IEnumerator RagdollizeBehavior()
    {
        yield return new WaitForSeconds(delay);

        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = false;

        foreach (Rigidbody r in rigidbodies)
            r.AddForce(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), ForceMode.VelocityChange);

        animator.enabled = false;
    }
}
