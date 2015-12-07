using UnityEngine;
using System.Collections;

public static class GameInput
{
    public static float GetAxis(string name, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return 0.0f;
        return Input.GetAxis(name);
    }

    public static float GetAxisRaw(string name, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return 0.0f;
        return Input.GetAxisRaw(name);
    }

    public static bool GetButton(string name, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return false;
        return Input.GetButton(name);
    }

    public static bool GetButtonDown(string name, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return false;
        return Input.GetButtonDown(name);
    }

    public static bool GetButtonUp(string name, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return false;
        return Input.GetButtonUp(name);
    }

    public static bool GetKey(KeyCode keycode, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return false;
        return Input.GetKey(keycode);
    }

    public static bool GetKeyDown(KeyCode keycode, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return false;
        return Input.GetKeyDown(keycode);
    }

    public static bool GetKeyUp(KeyCode keycode, bool realInput = false)
    {
        if (UIStateManager.State == UIState.Busy && !realInput)
            return false;
        return Input.GetKeyUp(keycode);
    }
}
