using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class UsableButton : MonoBehaviour, IUsable
{
    public string actionName = "";
    public string actionDescription = "";

    [System.Serializable]
    public class ButtonUseEvent : UnityEvent<InteractManager> { }

    public ButtonUseEvent onUse = new ButtonUseEvent();

    public virtual string GetActionName()
    {
        return actionName;
    }

    public virtual string GetDescription()
    {
        return actionDescription;
    }

    public virtual void Use(InteractManager user, UsableArgs args = null)
    {
        onUse.Invoke(user);
    }
}
