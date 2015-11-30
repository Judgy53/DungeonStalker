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
        int layerMask = ~(1 << LayerMask.NameToLayer("FirstPass"));

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, layerMask))
        {
            IUsable usable = hit.collider.GetComponentInParent<IUsable>();

            if (usable != null && (usable as Behaviour).enabled)
                target = usable;
            else
                target = null;
        }
        else
        {
            target = null;
        }

        if (target != null && Input.GetKeyDown(KeyCode.E) && Time.timeScale != 0.0f)
            target.Use(this);

        Debug.DrawLine(transform.position, transform.position + transform.forward * maxDistance, Color.red);
    }
}
