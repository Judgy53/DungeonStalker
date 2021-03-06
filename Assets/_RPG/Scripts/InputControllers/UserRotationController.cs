﻿using UnityEngine;
using System.Collections;

public class UserRotationController : MonoBehaviour, IRotationControls, ISavable
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
        mouseInputs = new Vector2(GameInput.GetAxis("Mouse X"), GameInput.GetAxis("Mouse Y"));
        if (target != null)
            rotation = target.localEulerAngles;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 frameVelocity = new Vector3(-mouseInputs.y * sensivity.y, mouseInputs.x * sensivity.x, 0.0f);
            rotation += frameVelocity * Time.fixedDeltaTime;

            float tempRotation = rotation.x - 180.0f;
            if (tempRotation > -110.0f && tempRotation < 0.0f)
                rotation.x = -110.0f + 180.0f;
            else if (tempRotation < 110.0f && tempRotation > 0.0f)
                rotation.x = 110.0f + 180.0f;

            target.localEulerAngles = rotation;
        }
    }

    public void Save(SaveData data)
    {
        rotation.ToSaveData(data, "Rotation");
    }

    public void Load(SaveData data)
    {
        rotation = new Vector3().FromSaveData(data, "Rotation");
    }
}
