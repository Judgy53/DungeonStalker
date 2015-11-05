using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    private Dictionary<string, string> datas = new Dictionary<string, string>();

    public void Add(string key, string value)
    {
        if (datas.ContainsKey(key))
            Debug.LogWarning("Saving : \"" + key + "\" has already been saved, overriding current value.");

        datas.Add(key, value);
    }

    public void Add(string key, object value)
    {
        datas.Add(key, value.ToString());
    }

    public string Get(string key)
    {
        if(datas.ContainsKey(key))
            return datas[key];

        return null;
    }
}
