using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IItem
{
    Sprite Image { get; }
    string Name { get; }
    uint Weigth { get; }

    bool CanDrop { get; set; }
    GameObject DropPrefab { get; }
}
