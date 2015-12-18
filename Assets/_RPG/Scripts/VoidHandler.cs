using UnityEngine;
using System.Collections;

public class VoidHandler : MonoBehaviour
{
    void OnTriggerStay(Collider collider)
    {
        if(collider.tag == "Player")
        {
            GameObject start = GameObject.FindGameObjectWithTag("PlayerStart");

            if (start == null)
                return;

            collider.transform.position = start.transform.position;
            collider.transform.rotation = start.transform.rotation;
        }
    }
}