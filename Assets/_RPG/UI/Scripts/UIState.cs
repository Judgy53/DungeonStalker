using UnityEngine;
using System.Collections;

public static class UIStateManager
{
    private static int count = 0;
    private static UIState state = UIState.Free;
    public static UIState State { get { return state; } }

    public static void RegisterUI()
    {
        count++;
        if (count > 0)
            state = UIState.Busy;
        else
            state = UIState.Free;
    }

    public static void UnregisterUI()
    {
        count = Mathf.Max(0, count - 1);

        if (count > 0)
            state = UIState.Busy;
        else
            state = UIState.Free;
    }
}

public enum UIState
{
    Free,
    Busy
}
