using UnityEngine;
using System.Collections;

public class UIPauseMenu : MonoBehaviour
{
    private bool state = false;

    [SerializeField]
    private GameObject content;

    private void Start()
    {
        Show(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if ((!state && UIStateManager.State == UIState.Free) || state)
                Show(!state);
        }
    }

    private void Show(bool newState = false)
    {
        state = newState;

        content.SetActive(newState);
        Time.timeScale = newState ? 0f : 1f;

        if (state)
            UIStateManager.RegisterUI();
        else
            UIStateManager.UnregisterUI();
    }

    public void MainMenu()
    {
        SaveManager.Instance.Save();
        Application.LoadLevel("MainMenu");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
