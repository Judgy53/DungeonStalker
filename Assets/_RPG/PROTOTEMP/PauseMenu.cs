using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
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

        Cursor.visible = newState;

        content.SetActive(newState);
        Time.timeScale = newState ? 0f : 1f;

        if (state)
            UIStateManager.RegisterUI();
        else
            UIStateManager.UnregisterUI();
    }

    public void Save()
    {
        SaveManager.Instance.Save();
        Show(false);
    }

    public void Load()
    {
        SaveManager.Instance.LoadLast();
        Show(false);
    }

    public void MainMenu()
    {
        Application.LoadLevel(0);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
