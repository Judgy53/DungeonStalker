using UnityEngine;
using System.Collections;

public interface ISavable
{

    void Save(SaveData data);

    void Load(SaveData data);
}
