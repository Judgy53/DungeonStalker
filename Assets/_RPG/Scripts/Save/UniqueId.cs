using UnityEngine;
using System.Collections;

public class UniqueIdAttribute : PropertyAttribute { }

public class UniqueId : MonoBehaviour
{
    [UniqueId]
    public string uniqueId;
}
