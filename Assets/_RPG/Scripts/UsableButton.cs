using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class UsableButton : MonoBehaviour, IUsable
{
    public string actionName = "";
    public string actionDescription = "";

    public UnityEvent onUse = new UnityEvent();

    public delegate string GetStringDelegate();
    public GetStringDelegate getActionNameDelegate;

    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        return actionDescription;
    }

    public void Use(InteractManager user, UsableArgs args = null)
    {
        onUse.Invoke();
    }
}
