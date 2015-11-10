using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    private Dictionary<string, string> datas = new Dictionary<string, string>();

    [NonSerialized]
    private string prefix = "";
    /// <summary>Add a prefix before keys. Use Carefully.</summary>
    public string Prefix
    {
        get
        {
            if (prefix == null)
                prefix = "";

            return prefix;
        }
        set
        {
            if (value != null)
                prefix = value;
            else
                prefix = "";
        }
    }

    public void Add(string key, string value)
    {
        if (datas.ContainsKey(Prefix + key))
            Debug.LogWarning("Saving : \"" + Prefix + key + "\" has already been saved, overriding current value.");
        
        datas.Add(Prefix + key, value);
    }

    public void Add(string key, object value)
    {
        Add(key, value.ToString());
    }

    public string Get(string key)
    {
        if (datas.ContainsKey(Prefix + key))
            return datas[Prefix + key];

        return null;
    }
}
