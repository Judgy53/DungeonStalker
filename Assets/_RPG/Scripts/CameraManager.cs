using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}