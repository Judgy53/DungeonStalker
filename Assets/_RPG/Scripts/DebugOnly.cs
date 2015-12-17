using UnityEngine;
using System.Collections;

public class DebugOnly : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        if (!Debug.isDebugBuild)
            Destroy(gameObject);
    }
}