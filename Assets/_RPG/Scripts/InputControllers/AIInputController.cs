using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Unit))]
public class AIInputController : MonoBehaviour, IControls
{
    private Vector3 moveSpeed = new Vector3(5.0f, 0.0f, 5.0f);
    public Vector3 MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    public GameObject Target 
    {
        get
        {
            if (unit.Target != null)
                return unit.Target.gameObject;
            return null;
        } 

        set { unit.Target = value.transform; } }

    public Vector3 Velocity { get { return unit.Velocity; } set { unit.Velocity = value; } }

    public float attackDistance = 0.5f;

    private WeaponManager weaponManager = null;

    private Unit unit = null;

    private Sensor sensor = null;

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        unit = GetComponent<Unit>();

        sensor = GetComponentInChildren<Sensor>();
        if (sensor != null)
            sensor.OnDetect += OnSensorDetect;
    }

    private void LateUpdate()
    {
        if (Target == null)
            return;

        if (Vector3.Distance(transform.position, Target.transform.position) <= attackDistance)
        {
            weaponManager.Primary(0);
            if (weaponManager.OffHandWeapon != null)
                weaponManager.Primary(1);
        }
    }

    private void OnSensorDetect(object sender, DetectSensorEvent e)
    {
        if (e.other.transform.parent.gameObject.tag == "Player" && Target == null)
            Target = e.other.gameObject;
    }
}
