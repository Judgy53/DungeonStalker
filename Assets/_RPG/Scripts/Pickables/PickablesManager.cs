using UnityEngine;
using System.Collections.Generic;

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
