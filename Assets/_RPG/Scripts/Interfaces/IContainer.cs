using UnityEngine;
using System.Collections;

public interface IContainer
{
    float MaxWeight { get; set; }

    IItem[] Items { get; }

    bool AddItem(IItem item);
    void RemoveItem(IItem item);
}
