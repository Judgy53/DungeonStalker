using UnityEngine;
using System.Collections;

public class TargetMover : MonoBehaviour
{
    public float speed = 20.0f;

    void Update()
    {
        Vector3 velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            velocity.z += speed;
        if (Input.GetKey(KeyCode.S))
            velocity.z -= speed;

        if (Input.GetKey(KeyCode.A))
            velocity.x -= speed;
        if (Input.GetKey(KeyCode.D))
            velocity.x += speed;

        transform.Translate(velocity * Time.deltaTime);
    }
}
