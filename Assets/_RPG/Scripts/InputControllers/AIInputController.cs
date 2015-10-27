using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AIInputController : MonoBehaviour, IControls
{
    [SerializeField]
    private Vector3 moveSpeed = new Vector3(5.0f, 0.0f, 5.0f);
    public Vector3 MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    public GameObject target = null;

    public float angleInterpolationFactor = 15.0f;

    private WeaponManager weaponManager = null;

    public Vector3 Velocity
    {
        get
        {
            if (navMesh != null)
                return navMesh.velocity;
            return Vector3.zero;
        }
        set 
        {
            if (navMesh != null)
                navMesh.velocity = value;
        }
    }

    private NavMeshAgent navMesh = null;

    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.speed = moveSpeed.x;

        target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
            navMesh.SetDestination(target.transform.position);

        weaponManager = GetComponent<WeaponManager>();
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            navMesh.SetDestination(target.transform.position);

            if (Vector3.Distance(transform.position, target.transform.position) <= navMesh.stoppingDistance)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation((target.transform.position - transform.position).normalized, Vector3.up),
                    angleInterpolationFactor * Time.fixedDeltaTime);

                weaponManager.Primary(1);
            }
        }
    }
}
