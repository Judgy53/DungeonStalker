using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

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

    private List<Save> saves = new List<Save>();
    public List<Save> Saves 
    { 
        get 
        {
            if (!Loaded)
                LoadAllSaves();

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

        try
        {
            Save sav = bformatter.Deserialize(stream) as Save;
            saves.Add(sav);
        }
        catch(Exception e)
        {
            Debug.LogError("Loading Error : " + e.Message);
        }

        stream.Close();
    }

    public void LoadLast()
    {
        SortSavesByDate(); // not needed but secure

        Load(Saves[0]);
    }

    public void Load(Save save)
    {
        save.Load();
    }

    public void Save()
    {
        if (!Loaded)
            LoadAllSaves();

        string fileName = "save" + saves.Count + ".sav";
        Directory.CreateDirectory(folder);
        Stream stream = File.Open(folder + fileName, FileMode.Create);
        BinaryFormatter bformatter = new BinaryFormatter();

        Save sav = new Save();
        sav.SaveCurrentSceneState();

        try
        {
            bformatter.Serialize(stream, sav);
            saves.Add(sav);
        }
        catch (Exception e)
        {
            Debug.LogError("Saving Error : " + e.Message);
        }

        stream.Close();
    }

    public void SortSavesByDate()
    {
        saves.Sort((s1, s2) => s1.creationDate.CompareTo(s2.creationDate));
        saves.Reverse(); // invert it to get most recent first
    }
}
