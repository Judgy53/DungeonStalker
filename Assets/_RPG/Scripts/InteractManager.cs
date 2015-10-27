using UnityEngine;
using System.Collections;

public class InteractManager : MonoBehaviour
{

    private IUsable target = null;
    public IUsable Target { get { return target; } }

    [SerializeField]
    private float maxDistance = 3.0f;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            IUsable usable = hit.collider.GetComponentInParent<IUsable>();

            target = usable;
        }
        else
        {
            target = null;
        }

        if (target != null && Input.GetKeyDown(KeyCode.E))
        {
            target.Use();
        }

        Debug.DrawLine(transform.position, transform.position + transform.forward * maxDistance, Color.red);
    }
}
