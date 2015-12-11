using UnityEngine;
using System.Collections;

public class OnPlayerDeath : MonoBehaviour
{
    private HealthManager healthManager = null;

    private UserInputController inputController = null;
    private UserRotationController rotationController = null;

    void Start()
    {
        healthManager = GetComponent<HealthManager>();
        inputController = GetComponent<UserInputController>();
        rotationController = GetComponent<UserRotationController>();

        if (healthManager != null)
            healthManager.OnDeath += healthManager_OnDeath;
    }

    void healthManager_OnDeath(object sender, System.EventArgs e)
    {
        if (inputController != null)
            inputController.enabled = false;

        if (rotationController != null)
            rotationController.enabled = false;

        SaveManager.Instance.DeleteSave(GameManager.GameId);
    }
}