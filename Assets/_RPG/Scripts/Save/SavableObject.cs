using UnityEngine;
using System.Collections;

public class UniqueIdAttribute : PropertyAttribute { }

public class SavableObject : MonoBehaviour
{
    [UniqueId]
    public string uniqueId;

    /// <summary>
    /// Wait for the whole level to load before loading this
    /// </summary>
    [Tooltip("Wait for the whole level to load before loading this")]
    public bool WaitPlayerCreationToLoad = false;
}
