using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Save
{
    //store informations outside dictionnary for easy access
    public string GameId = null;
    public string PlayerName = "Player";
    public uint PlayerLevel = 0;
    public uint Stage = 0;
    public DateTime CreationDate;
    public int TimePlayed = 0; // in seconds

    private Dictionary<string, SaveData> saveDatas = new Dictionary<string, SaveData>();
    private Dictionary<string, SaveData> delayedDatas = new Dictionary<string, SaveData>();

    public void SaveGame()
    {
        GameId = GameManager.GameId;
        PlayerName = GameManager.PlayerName;
        PlayerLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsManager>().CurrentLevel;
        Stage = GameManager.Stage;
        CreationDate = DateTime.Now;
        TimePlayed = GameManager.TimePlayed;
        
        SavableObject[] saveables = GameObject.FindObjectsOfType<SavableObject>();

        if (saveables.Length == 0) // nothing to save
            return;
        
        foreach (SavableObject savGaO in saveables)
        {
            ISavable[] save = savGaO.GetComponents<ISavable>();

            if (save.Length > 0)
            {
                string dataId = savGaO.uniqueId;
                SaveData data = CreateSaveData(save);

                if (savGaO.WaitPlayerCreationToLoad)
                    delayedDatas.Add(dataId, data);
                else
                    saveDatas.Add(dataId, data);
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
        GameManager.GameId = GameId;
        GameManager.Stage = Stage;
        GameManager.ResetTime(TimePlayed);
        GameManager.PlayerName = PlayerName;

        LoadDatas(saveDatas);

        Debug.Log("Save Data Loaded");

        if(delayedDatas.Count > 0)
            GameManager.OnPlayerCreation += DelayedLoad; // PlayerCreation should be the last action in level creation
    }

    private void DelayedLoad(object sender, EventArgs e)
    {
        GameManager.OnPlayerCreation -= DelayedLoad;

        LoadDatas(delayedDatas);

        Debug.Log("Delayed Save Data Loaded");
    }

    private void LoadDatas(Dictionary<string, SaveData> datas)
    {
        SavableObject[] ids = GameObject.FindObjectsOfType<SavableObject>();

        if (ids.Length == 0)
            return;

        foreach (SavableObject savGaO in ids)
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
