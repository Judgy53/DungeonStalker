using UnityEngine;
using System.Collections;

public class CollisionsMessageRecovery : MonoBehaviour
{
    public LayerMask layerMask = -1;
    public float skinWidth = 0.1f;
    public bool restorePosition = false;

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Collider[] colliders = new Collider[0];
    private new Rigidbody rigidbody = null;

    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        rigidbody = GetComponentInChildren<Rigidbody>();

        previousPosition = rigidbody.position;

        Vector3 highestExtents = Vector3.zero;
        foreach (Collider c in colliders)
        {
            if (highestExtents.x < c.bounds.extents.x)
                highestExtents.x = c.bounds.extents.x;
            if (highestExtents.y < c.bounds.extents.y)
                highestExtents.y = c.bounds.extents.y;
            if (highestExtents.z < c.bounds.extents.z)
                highestExtents.z = c.bounds.extents.z;
        }

        minimumExtent = Mathf.Min(Mathf.Min(highestExtents.x, highestExtents.y), highestExtents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void FixedUpdate()
    {
        Vector3 movementThisStep = rigidbody.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", colliders[0]);

                if (restorePosition && hitInfo.collider.enabled && !hitInfo.collider.isTrigger)
                    rigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;

            }
        }

        previousPosition = rigidbody.position;
    }
}
