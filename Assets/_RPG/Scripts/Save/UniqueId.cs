using UnityEngine;
using System.Collections;

public class UniqueIdAttribute : PropertyAttribute { }

public class UniqueId : MonoBehaviour
{
    [UniqueId]
    public string uniqueId;

    /// <summary>
    /// Wait for the whole level to load before loading this
    /// </summary>
    public bool DelayedLoad = false;
}
