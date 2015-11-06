using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Sensor : MonoBehaviour
{
    public event EventHandler<DetectSensorEvent> OnDetect;

    private void OnTriggerStay(Collider other)
    {
        Ray ray = new Ray(transform.position, other.transform.position - transform.position);
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position, other.transform.position);
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider == other && OnDetect != null)
                OnDetect(this, new DetectSensorEvent(other));
        }
    }
}

public class DetectSensorEvent : EventArgs
{
    public Collider other;

    public DetectSensorEvent(Collider other)
    {
        this.other = other;
    }
}