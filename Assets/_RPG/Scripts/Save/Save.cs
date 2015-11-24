using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Save
{
    //store informations outside dictionnary for easy access
    public string PlayerName = "Player";
    public uint PlayerLevel = 0;
    public uint Stage = 0;
    public DateTime CreationDate;
    public long TimePlayed = 0L; // in seconds

    private Dictionary<string, SaveData> datas = new Dictionary<string, SaveData>();

    public void SaveScene()
    {
        PlayerName = GameManager.PlayerName;
        PlayerLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsManager>().CurrentLevel;
        Stage = GameManager.Stage;
        CreationDate = DateTime.Now;
        TimePlayed = GameManager.TimePlayed;
        
        UniqueId[] saveables = GameObject.FindObjectsOfType<UniqueId>();

        if (saveables.Length == 0) // nothing to save
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
        GameManager.Stage = Stage;
        GameManager.ResetTime(TimePlayed);
        GameManager.PlayerName = PlayerName;

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

            SaveData data = datas[dataId];
            data.Prefix = ""; //no prefix when loading starts

            foreach (ISavable sav in savables)
                sav.Load(data);
        }
    }

}
