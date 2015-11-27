using UnityEngine;
using System.Collections;

public interface IContainer
{
    float MaxWeight { get; set; }
    float CurrentWeight { get; }

    IItem[] Items { get; }

    bool AddItem(IItem item);
    bool RemoveItem(IItem item);
}
