using UnityEngine;
using System.Collections;

public class UISaveMenu : MonoBehaviour
{
    private SaveLoadState state = SaveLoadState.None;
    public SaveLoadState State { get { return state; } set { state = value; } }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        UIStateManager.RegisterUI();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        UIStateManager.UnregisterUI();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        UIStateManager.UnregisterUI();
    }
}

public enum SaveLoadState
{
    None = 0,
    Save, 
    Load
}