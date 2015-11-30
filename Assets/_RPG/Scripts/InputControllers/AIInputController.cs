using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
public class AIInputController : MonoBehaviour, IControls, ISavable
{
    public delegate void AIBehavior(AIInputController controller);

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

        set { unit.Target = value.transform; } 
    }

    public Vector3 Velocity { get { return unit.Velocity; } set { unit.Velocity = value; } }

    public bool IsGrounded { get { return unit.IsGrounded; } }
    
    public float attackDistance = 0.5f;
    public bool canBackpedal = false;
    public float safetyZone = 2.0f;
    public float runZone = 0.0f;

    public AIBehavior behavior = null;

    private WeaponManager weaponManager = null;
    public WeaponManager WeaponManager { get { return weaponManager; } }

    private Unit unit = null;
    public Unit Unit { get { return unit; } }

    private Sensor sensor = null;
    public Sensor Sensor { get { return sensor; } }

    private AnimationDriverBase driver = null;
    public AnimationDriverBase Driver { get { return driver; } }

    private float targetDistance = 0.0f;
    public float TargetDistance { get { return targetDistance; } }

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        unit = GetComponent<Unit>();

        driver = GetComponent<AnimationDriverBase>();

        sensor = GetComponentInChildren<Sensor>();
        if (sensor != null)
            sensor.OnDetect += OnSensorDetect;
    }


    private void FixedUpdate()
    {
        EvaluateSituation();
        if (behavior != null)
            behavior(this);

        Vector3 lsVelocity = transform.InverseTransformDirection(Velocity);
        driver.SetMovementVelocity(lsVelocity.z / unit.MoveSpeed.z, lsVelocity.x / unit.MoveSpeed.x, lsVelocity.y / unit.MoveSpeed.y, IsGrounded);
    }

    private void OnSensorDetect(object sender, DetectSensorEvent e)
    {
        if (e.other.transform.parent.gameObject.tag == "Player" && Target == null)
            Target = e.other.gameObject;
    }

    public void Save(SaveData data)
    {
        transform.position.ToSaveData(data, "Position");
        transform.eulerAngles.ToSaveData(data, "Rotation");
    }

    public void Load(SaveData data)
    {
        transform.position = new Vector3().FromSaveData(data, "Position");
        transform.rotation = Quaternion.Euler(new Vector3().FromSaveData(data, "Rotation"));
    }

    private void EvaluateSituation()
    {
        if (Target != null)
        {
            targetDistance = Vector3.Distance(transform.position, Target.transform.position);
            if (targetDistance < attackDistance)
            {
                if (!canBackpedal)
                    behavior = AIBehaviorExtensions.GetInRangeAndAttackBehavior;
                else if (targetDistance < attackDistance - safetyZone && sensor.GotVisual)
                    behavior = AIBehaviorExtensions.BackpedalBehavior;
                else
                    behavior = AIBehaviorExtensions.GetInRangeAndAttackBehavior;
            }
            else
                behavior = AIBehaviorExtensions.GetInRangeAndAttackBehavior;
        }
    }
}

public static class AIBehaviorExtensions
{
    public static void GetInRangeAndAttackBehavior(AIInputController controller)
    {
        if (controller.Target != null && controller.Unit.Path != null && controller.Unit.Path.Length != 0)
        {
            Vector3 dir = controller.Unit.DirectionToNextPoint;
            if (dir != Vector3.zero)
            {
                controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation,
                    Quaternion.LookRotation(dir),
                    controller.Unit.angleInterpolationFactor * Time.fixedDeltaTime);
            }

            if (controller.TargetDistance > controller.attackDistance)
                controller.Unit.ForwardInput = Mathf.Lerp(controller.Unit.ForwardInput, 1.0f, 1.0f * Time.deltaTime);
            else
            {
                controller.WeaponManager.Primary(0);
                if (controller.WeaponManager.OffHandWeapon != null)
                    controller.WeaponManager.Primary(1);

                controller.Unit.ForwardInput = Mathf.Lerp(controller.Unit.ForwardInput, 0.0f, 3.0f * Time.deltaTime);
            }
        }
        else
            controller.Unit.ForwardInput = Mathf.Lerp(controller.Unit.ForwardInput, 0.0f, 3.0f * Time.deltaTime);
    }

    public static void BackpedalBehavior(AIInputController controller)
    {
        if (controller.Target != null)
        {
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation,
                Quaternion.LookRotation(controller.Unit.DirectionToTarget),
                controller.Unit.angleInterpolationFactor * Time.fixedDeltaTime);

            if (controller.TargetDistance < controller.attackDistance)
            {
                controller.WeaponManager.Primary(0);
                if (controller.WeaponManager.OffHandWeapon != null)
                    controller.WeaponManager.Primary(1);
            }

            controller.Unit.ForwardInput = Mathf.Lerp(controller.Unit.ForwardInput, -1.0f, 2.0f * Time.deltaTime);
        }
    }

    public static void FleeBehavior(AIInputController controller)
    {
    }
}
