using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{
    public float speed = 20.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
