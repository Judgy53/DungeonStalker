using UnityEngine;
using System.Collections;
using System;

public class PlayerAimGatherer : MonoBehaviour, IAimGatherer
{
    public Vector3 GetAimHitpoint(ref IDamageable dmg)
    {
        RaycastHit hit;
        Vector3 ret;
        if (Camera.main.GetWorldHitpoint(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f), out hit, out ret))
            dmg = hit.collider.gameObject.GetComponentInParent<IDamageable>();
        return ret;
    }
}
