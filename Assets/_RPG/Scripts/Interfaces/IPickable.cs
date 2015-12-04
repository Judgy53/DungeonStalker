using UnityEngine;
using System.Collections;

public interface IPickable : IUsable
{
    GameObject PickedItemPrefab { get; }

    object UserData { get; set; }
}
