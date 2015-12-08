using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class PickablesManager : MonoBehaviour, ISavable
{
    private List<Pickable> pickables = new List<Pickable>();

    private static PickablesManager instance = null;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public static void RegisterPickable(Pickable pickable)
    {
        if(instance == null)
        {
            Debug.LogError("Can't register pickable, PickablesManager not initialized");
            return;
        }

        if(pickable == null)
        {
            Debug.LogWarning("Trying to register a null pickable. Registering cancelled.");
            return;
        }

        instance.pickables.Add(pickable);
    }

    public void Save(SaveData data)
    {
        pickables.RemoveAll(x => x == null);

        int count = pickables.Count;

        data.Add("count", count);

        for (int i = 0; i < count; i++ )
        {
            Pickable pick = pickables[i];

            data.Prefix = "pick_" + i + "_";

            pick.transform.position.ToSaveData(data, "pos");
            pick.transform.rotation.eulerAngles.ToSaveData(data, "rot");

            string path = ResourcesPathHelper.GetPickablePath(pick);

            if (path != null)
                data.Add("path", path);

            //UserData can be anything, serializing it to string to save it
            if(pick.UserData != null && pick.UserData.GetType().IsSerializable)
            {
                StringWriter writer = new StringWriter();
                XmlSerializer xmlSerializer = new XmlSerializer(pick.UserData.GetType()); // We Use XML Serializer to be able to write in a StringWriter

                xmlSerializer.Serialize(writer, pick.UserData);

                data.Add("userDataType", pick.UserData.GetType().ToString());
                data.Add("userData", writer.ToString());
            }
        }
    }

    public void Load(SaveData data)
    {
        ClearList();

        int count = int.Parse(data.Get("count"));

        for (int i = 0; i < count; i++)
        {
            data.Prefix = "pick_" + i + "_";

            string path = data.Get("path");

            if (path == null)
                continue;

            Vector3 pos = new Vector3().FromSaveData(data, "pos");
            Quaternion rot = Quaternion.Euler(new Vector3().FromSaveData(data, "rot"));

            GameObject prefab = Resources.Load(path) as GameObject;
            if(prefab == null)
            {
                Debug.LogWarning("Loading Pickables : Failed to load \"" + path + "\"");
                continue;
            }

            GameObject instance = Instantiate(prefab, pos, rot) as GameObject;

            instance.transform.parent = transform;

            string userDataType = data.Get("userDataType");

            //UserData can be anything, deserializing it from string to load it
            if(userDataType != null)
            {
                Pickable pick = instance.GetComponent<Pickable>();

                System.Type dataType = System.Type.GetType(userDataType);
                string userDataSerialized = data.Get("userData");

                StringReader textReader = new StringReader(userDataSerialized);
                XmlSerializer xmlSerializer = new XmlSerializer(dataType);

                pick.UserData = xmlSerializer.Deserialize(textReader);
            }
        }
    }

    private void ClearList()
    {
        foreach (Pickable pick in pickables)
        {
            Destroy(pick.gameObject);
        }

        pickables.Clear();
    }
}
