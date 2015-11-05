using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CharacterController))]
public class Unit : MonoBehaviour, IControls
{
    public Transform target = null;
    public float speed = 20.0f;
    public float precision = 2.0f;
    public float angleInterpolationFactor = 5.0f;
    public float acceleration = 0.5f;

    public float gravityMultiplier = 1.0f;

    public float actualizationTime = 0.3f;

    private Vector3[] path = null;
    private int targetIndex = 0;
    private bool pathProcessing = false;

    private Vector3 oldTargetPosition = Vector3.zero;

    [SerializeField]
    private Vector3 moveSpeed = new Vector3(5.0f, 0.0f, 5.0f);
    public Vector3 MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

    private CharacterController cc = null;

    private float forwardInput = 0.0f;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        StartCoroutine(AutoActualizePath());
    }

    private void FixedUpdate()
    {
        if (path != null && path.Length != 0 && targetIndex < path.Length)
        {
            Vector3 currentWaypoint = path[targetIndex];

            if (Vector3.Distance(transform.position, currentWaypoint) <= precision)
            {
                targetIndex++;

                if (targetIndex >= path.Length)
                    return;

                currentWaypoint = path[targetIndex];
            }

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation((currentWaypoint - transform.position).normalized, Vector3.up),
                angleInterpolationFactor * Time.fixedDeltaTime);

            /*if (effectManager != null)
            {
                IMovementEffect[] effects = effectManager.GetEffects<IMovementEffect>();
                foreach (IMovementEffect e in effects)
                    e.ApplyMovementEffect(ref frameVelocity);
            }*/

            forwardInput = Mathf.Lerp(forwardInput, 1.0f, 0.5f * Time.fixedDeltaTime);
        }
        else
            forwardInput = Mathf.Lerp(forwardInput, 0.0f, 5.0f * Time.fixedDeltaTime);

        Vector3 frameVelocity = moveSpeed;
        frameVelocity.Scale(new Vector3(0.0f, 1.0f, forwardInput));

        float velocityy = velocity.y;
        velocity = transform.TransformDirection(frameVelocity);
        velocity.y = velocityy;

        velocity += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    private void OnPathFound(Vector3[] newPath, bool success)
    {
        if (success)
            path = newPath;
        else
        {
            Debug.Log("No path availiable");
            path = null;
        }

        pathProcessing = false;

        oldTargetPosition = target.position;

        targetIndex = 0;
    }

    IEnumerator AutoActualizePath()
    {
        while (true)
        {
            if (oldTargetPosition != target.position)
            {
                if (!pathProcessing)
                {
                    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                    pathProcessing = true;
                }
            }
            yield return new WaitForSeconds(actualizationTime);
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
    }
}
