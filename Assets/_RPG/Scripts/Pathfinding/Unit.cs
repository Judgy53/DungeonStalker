using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CharacterController))]
public class Unit : MonoBehaviour, IControls
{
    [SerializeField]
    private Transform target = null;
    public Transform Target
    {
        get { return target; }
        set
        {
            StopCoroutine(AutoActualizePath());
            target = value;
            path = null;
            if (target != null)
                StartCoroutine(AutoActualizePath());
        }
    }

    public float precision = 2.0f;
    public float angleInterpolationFactor = 5.0f;
    public float acceleration = 0.5f;

    public float gravityMultiplier = 1.0f;

    public float actualizationTime = 0.3f;

    public float gizmoSize = 1.0f;

    private Vector3[] path = null;
    private int targetIndex = 0;
    private bool pathProcessing = false;

    private Vector3 oldTargetPosition = Vector3.zero;

    [SerializeField]
    private Vector3 moveSpeed = new Vector3(5.0f, 0.0f, 5.0f);
    public Vector3 MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

    public Vector3 DirectionToTarget 
    { 
        get 
        {
            if (target == null)
                return Vector3.zero;

            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0.0f;
            return direction.normalized;
        }
    }

    public Vector3 DirectionToNextPoint
    {
        get
        {
            Vector3 direction = Vector3.zero;
            if (path != null && path.Length != 0 && targetIndex < path.Length)
                direction = path[targetIndex] - transform.position;
            else if (path != null && targetIndex >= path.Length)
                direction = path[path.Length - 1] - transform.position;

            direction.y = 0.0f;

            return direction;
        }
    }

    public bool PathComplete { get { return path == null || path.Length == 0 || targetIndex >= path.Length; } }

    public bool IsGrounded { get { return cc.isGrounded; } }

    private CharacterController cc = null;

    private float forwardInput = 0.0f;
    public float ForwardInput { get { return forwardInput; } set { forwardInput = Mathf.Clamp(value, -1.0f, 1.0f); } }

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
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

            /*Vector3 direction = currentWaypoint - transform.position;
            direction.y = 0.0f;
            direction.Normalize();

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(direction),
                angleInterpolationFactor * Time.fixedDeltaTime);*/
            
            /*if (effectManager != null)
            {
                IMovementEffect[] effects = effectManager.GetEffects<IMovementEffect>();
                foreach (IMovementEffect e in effects)
                    e.ApplyMovementEffect(ref frameVelocity);
            }*/

            //forwardInput = Mathf.Lerp(forwardInput, 1.0f, 0.5f * Time.fixedDeltaTime);
        }
        //else
            //forwardInput = Mathf.Lerp(forwardInput, 0.0f, 5.0f * Time.fixedDeltaTime);

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
            path = null;

        pathProcessing = false;

        if (target != null)
            oldTargetPosition = target.position;

        targetIndex = 0;
    }

    IEnumerator AutoActualizePath()
    {
        while (true)
        {
            if (target != null && enabled)
            {
                if (oldTargetPosition != target.position)
                {
                    if (!pathProcessing)
                    {
                        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                        pathProcessing = true;
                    }
                }
            }

            yield return new WaitForSeconds(actualizationTime);
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], new Vector3(gizmoSize, gizmoSize, gizmoSize));

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}
