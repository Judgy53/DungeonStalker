using UnityEngine;
using System;
using System.Collections;

public class OnPlayerDeath : MonoBehaviour
{

    void Start()
    {
        GetComponent<HealthManager>().OnDeath += OnDeath;
    }

    private void OnDeath(object sender, EventArgs args)
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
