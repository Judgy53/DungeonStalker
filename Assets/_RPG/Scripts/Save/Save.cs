using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Save
{
    public DateTime creationDate;
    private Dictionary<string, SaveData> datas = new Dictionary<string, SaveData>();

    public void SaveCurrentSceneState()
    {
        UniqueId[] saveables = GameObject.FindObjectsOfType<UniqueId>();

        if (saveables.Length == 0)
            return;

        foreach (UniqueId savGaO in saveables)
        {
            ISavable[] save = savGaO.GetComponents<ISavable>();

            if (save.Length > 0)
            {
                string dataId = savGaO.uniqueId;
                SaveData data = CreateSaveData(save);

                datas.Add(dataId, data);
            }
        }

        creationDate = DateTime.Now;
    }

    private SaveData CreateSaveData(ISavable[] save)
    {
        SaveData data = new SaveData();

        foreach (ISavable compData in save)
            compData.Save(data);

        return data;
    }

    public void Load()
    {
        UniqueId[] ids = GameObject.FindObjectsOfType<UniqueId>();

        if (ids.Length == 0)
            return;

        foreach (UniqueId savGaO in ids)
        {
            string dataId = savGaO.uniqueId;

            if (!datas.ContainsKey(dataId))
                continue;

            ISavable[] savables = savGaO.GetComponents<ISavable>();

            if (savables.Length == 0)
                continue;

            foreach (ISavable sav in savables)
            {
                SaveData data = datas[dataId];
                sav.Load(data);
            }
        }
    }

}
