using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UserInputController : MonoBehaviour, IControls, ISavable
{
    [SerializeField] 
    private Vector3 moveSpeed = new Vector3(5.0f, 1.0f, 5.0f);
    public Vector3 MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    public float sprintMultiplier = 2.5f;

    public float jumpForce = 5.0f;

    private CharacterController cc = null;
    public CharacterController CC { get { return cc; } }

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

    public bool IsGrounded { get { return cc.isGrounded; } }

    private Vector2 horizontalInputs = Vector2.zero;
    private float sprintInput = 0.0f;
    private bool jumpInput = false;

    [SerializeField]
    private float gravityMultiplier = 1.0f;
    public float GravityMultiplier { get { return gravityMultiplier; } set { gravityMultiplier = value; } }

    private WeaponManager weaponManager = null;

    private EffectManager effectManager = null;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        weaponManager = GetComponent<WeaponManager>();
        effectManager = GetComponent<EffectManager>();
    }

    private void Update()
    {
        horizontalInputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        sprintInput = Input.GetAxis("Sprint");
        jumpInput = Input.GetButtonDown("Jump");

        if (weaponManager != null)
        {
            if (Input.GetButtonDown("Fire1"))
                weaponManager.Primary(1);
            else if (Input.GetButtonUp("Fire1"))
                weaponManager.EndPrimary(1);

            if (Input.GetButtonDown("Fire2"))
                weaponManager.Secondary();
            else if (Input.GetButtonUp("Fire2"))
                weaponManager.EndSecondary();
        }

        
        if (Input.GetKeyDown(KeyCode.F5))
            SaveManager.Instance.Save();
        if (Input.GetKeyDown(KeyCode.F9))
            SaveManager.Instance.LoadLast();
        
    }

    private void FixedUpdate()
    {
        Vector3 frameVelocity = moveSpeed;
        frameVelocity.Scale(new Vector3(horizontalInputs.x, 1.0f, horizontalInputs.y));

        frameVelocity += frameVelocity * (sprintMultiplier * sprintInput);

        if (effectManager != null)
        {
            IMovementEffect[] effects = effectManager.GetEffects<IMovementEffect>();
            foreach (IMovementEffect e in effects)
                e.ApplyMovementEffect(ref frameVelocity);
        }

        

        float velocityy = velocity.y;
        //velocity = transform.TransformDirection(frameVelocity);
        velocity = frameVelocity.RotateAround(Vector3.zero, Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
        velocity.y = velocityy;

        if (jumpInput && cc.isGrounded)
            velocity.y = jumpForce;

        velocity += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    public void Save(SaveData data)
    {
        transform.position.ToSaveData(data, "Position");
        transform.eulerAngles.ToSaveData(data, "Rotation");
        velocity.ToSaveData(data, "Velocity");
    }

    public void Load(SaveData data)
    {
        transform.position = new Vector3().FromSaveData(data, "Position");
        transform.rotation = Quaternion.Euler(new Vector3().FromSaveData(data, "Rotation"));
        velocity = new Vector3().FromSaveData(data, "Velocity");
    }
}
