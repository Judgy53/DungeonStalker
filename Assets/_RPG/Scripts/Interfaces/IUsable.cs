using UnityEngine;
using System.Collections;

public interface IUsable
{

    string GetActionName();
    string GetDescription();

    void Use();
}
