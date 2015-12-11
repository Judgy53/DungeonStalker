using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject content = null;

    [SerializeField]
    private Button quitButton = null;

    private HealthManager health = null;

    private void Start()
    {
        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;

        content.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerCreation -= GameManager_OnPlayerCreation;
        
        if(health != null)
            health.OnDeath -= Player_OnDeath;
    }

    private void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        health = e.player.GetComponent<HealthManager>();

        if(health != null)
            health.OnDeath += Player_OnDeath;
    }

    private void Player_OnDeath(object sender, System.EventArgs e)
    {
        content.SetActive(true);

        UIStateManager.RegisterUI();

        quitButton.onClick.AddListener(CryQuit);
    }

    private void CryQuit()
    {
        GameManager.GoToMainMenu();
    }
}