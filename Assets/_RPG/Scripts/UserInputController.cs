using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UserInputController : MonoBehaviour, IControls
{
    [SerializeField] 
    private Vector3 moveSpeed = new Vector3(5.0f, 0.0f, 5.0f);
    public Vector3 MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    private CharacterController cc = null;
    public CharacterController CC { get { return cc; } }

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

    private Vector2 horizontalInputs = Vector2.zero;

    [SerializeField]
    private float gravityMultiplier = 1.0f;
    public float GravityMultiplier { get { return gravityMultiplier; } set { gravityMultiplier = value; } }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        horizontalInputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        Vector3 frameVelocity = moveSpeed;
        frameVelocity.Scale(new Vector3(horizontalInputs.x, 0.0f, horizontalInputs.y));
        velocity = transform.TransformDirection(frameVelocity * Time.fixedDeltaTime);

        velocity += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;

        cc.Move(velocity);
    }
}
