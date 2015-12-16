using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Sensor : MonoBehaviour
{
    public event EventHandler<DetectSensorEvent> OnDetect;

    private bool gotVisual = false;
    public bool GotVisual { get { return gotVisual; } }

    private void OnTriggerStay(Collider other)
    {
        Ray ray = new Ray(transform.position, other.transform.position - transform.position);
        Debug.DrawLine(transform.position, other.transform.position);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits,
            delegate (RaycastHit x, RaycastHit y)
            {
                float distX = Vector3.Distance(x.point, transform.position);
                float distY = Vector3.Distance(y.point, transform.position);
                if (distX < distY)
                    return -1;
                else if (distX > distY)
                    return 1;
                return 0;
            });

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == other)
                {
                    if (OnDetect != null)
                        OnDetect(this, new DetectSensorEvent(other));
                    gotVisual = true;
                    break;
                }
                else
                {
                    if (transform.IsChildOf(hit.collider.transform) || hit.collider.transform.IsChildOf(transform) || 
                        hit.collider.transform == transform || other.transform.IsChildOf(hit.transform))
                        continue;

                    gotVisual = false;
                    break;
                }
            }
        }
        else
            gotVisual = false;
    }

    private void OnTriggerExit(Collider other)
    {
        gotVisual = false;
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