using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text.RegularExpressions;

public class SaveManager {

    private static SaveManager instance = null;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SaveManager();

            return instance;
        }
    }

    private bool Loaded = false;

    private Dictionary<string, Save> saves = new Dictionary<string, Save>();
    public Dictionary<string, Save> Saves 
    { 
        get 
        {
            if (!Loaded)
                LoadAllSaves();

            SortSavesByDate();

            return saves; 
        } 
    }

    private string folder = "saves/";

    private SaveManager()
    {
        //Small security to ensure folder ends with a '/'
        if (!folder.EndsWith("/"))
            folder += "/";

        LoadAllSaves();
    }

    private void LoadAllSaves()
    {
        if (!Directory.Exists(folder))
            return;

        string[] savFiles = Directory.GetFiles(folder, "*.sav");

        saves.Clear();

        foreach(string savName in savFiles)
            LoadFile(savName);

        Loaded = true;
    }

    private void LoadFile(string saveFile)
    {
        if (!File.Exists(saveFile))
            return;

        Stream stream = File.Open(saveFile, FileMode.Open);
        BinaryFormatter bformatter = new BinaryFormatter();

        string cutFolder = Regex.Replace(saveFile, ".*/", "");
        string fileName = Regex.Replace(cutFolder, "\\.sav", "");

        try
        {
            Save sav = bformatter.Deserialize(stream) as Save;
            saves.Add(fileName, sav);
        }
        catch(Exception e)
        {
            Debug.LogError("Loading \"" + fileName + "\" Error : " + e.Message);
        }

        stream.Close();
    }

    public void LoadLast()
    {
        SortSavesByDate();

        Load(saves.Last().Value);
    }

    public void Load(Save save)
    {
        save.Load();
    }

    public void Load(string saveId)
    {
        string saveName = "save" + saveId;

        if (Saves.ContainsKey(saveName))
            Load(Saves[saveName]);
        else
            Debug.Log("Failed loading " + saveName + " : save doesn't exists");
    }

    public void Save()
    {
        if (!Loaded)
            LoadAllSaves();

        string fileName = GetSaveName();

        Directory.CreateDirectory(folder);
        Stream stream = File.Open(folder + fileName + ".sav", FileMode.Create);
        BinaryFormatter bformatter = new BinaryFormatter();

        Save sav = new Save();
        sav.SaveGame();

        try
        {
            bformatter.Serialize(stream, sav);
        }
        catch (Exception e)
        {
            Debug.LogError("Saving Error : " + e.Message);
        }

        if (saves.ContainsKey(fileName))
            saves.Remove(fileName);

        saves.Add(fileName, sav);

        stream.Close();

        SortSavesByDate();
    }

    private string GetSaveName()
    {
        string currentId = GameManager.GameId;

        foreach(KeyValuePair<string, Save> kvp in Saves)
        {
            if (kvp.Value.GameId.Equals(currentId))
                return kvp.Key;
        }

       return "save" + saves.Count.ToString("000");
    }

    public void SortSavesByDate()
    {
        saves = saves.OrderBy(d => d.Value.CreationDate).ToDictionary(x=>x.Key,x=>x.Value);
    }
}
