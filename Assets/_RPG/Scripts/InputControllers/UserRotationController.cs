using UnityEngine;
using System.Collections;

public class UserRotationController : MonoBehaviour, IRotationControls
{
    [SerializeField]
    private Vector2 sensivity = new Vector2(5.0f, 5.0f);
    public Vector2 Sensivity { get { return sensivity; } set { sensivity = value; } }

    private Vector3 rotation = Vector3.zero;
    public Vector3 Rotation { get { return rotation; } set { rotation = value; } }

    [SerializeField]
    private Transform target = null;
    public Transform Target { get { return target; } }

    private Vector2 mouseInputs = Vector2.zero;

    private void Update()
    {
        mouseInputs = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (target != null)
            rotation = target.localEulerAngles;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 frameVelocity = new Vector3(-mouseInputs.y * sensivity.y, mouseInputs.x * sensivity.x, 0.0f);
            rotation += frameVelocity * Time.fixedDeltaTime;

            target.localEulerAngles = rotation;
        }
    }
}
