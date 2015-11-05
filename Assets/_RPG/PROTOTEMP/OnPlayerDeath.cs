using UnityEngine;
using System;
using System.Collections;

public class OnPlayerDeath : MonoBehaviour
{

    void Start()
    {
        GetComponent<HealthManager>().OnKill += OnDeath;
    }

    private void OnDeath(object sender, EventArgs args)
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
