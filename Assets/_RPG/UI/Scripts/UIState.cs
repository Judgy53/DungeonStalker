using UnityEngine;
using System.Collections;

public static class UIStateManager
{
    private static int count = 0;
    public static UIState State 
    { 
        get 
        {
            if (count > 0)
                return UIState.Busy;

            return UIState.Free;
        } 
    }

    public static void RegisterUI()
    {
        count++;

        HandleCursorState();
    }

    public static void UnregisterUI()
    {
        count = Mathf.Max(0, count - 1);

        HandleCursorState();
    }

    public static void HandleCursorState()
    {
        Cursor.visible = count > 0;
        Cursor.lockState = count > 0 ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public static void ClearState()
    {
        count = 0;
    }
}

public enum UIState
{
    Free,
    Busy
}
