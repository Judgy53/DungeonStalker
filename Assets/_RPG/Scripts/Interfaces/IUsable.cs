using UnityEngine;
using System.Collections;

public interface IUsable
{
    string GetActionName();
    string GetDescription();

    void Use(InteractManager user, UsableArgs args = null);
}

public abstract class UsableArgs
{
}