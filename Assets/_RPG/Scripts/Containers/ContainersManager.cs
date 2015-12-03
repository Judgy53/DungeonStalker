using UnityEngine;
using System.Collections.Generic;

public class ContainersManager : MonoBehaviour, ISavable
{

    private List<Container> containers = new List<Container>();

    private static ContainersManager instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public static void RegisterContainer(Container container)
    {
        if (instance == null)
        {
            Debug.LogError("Can't register pickable, PickablesManager not initialized");
            return;
        }

        if (container == null)
        {
            Debug.LogWarning("Trying to register a null pickable. Registering cancelled.");
            return;
        }

        instance.containers.Add(container);
    }

    public void Save(SaveData data)
    {
        containers.RemoveAll(x => x == null);

        int count = containers.Count;

        data.Add("count", count);

        for (int i = 0; i < count; i++)
        {
            Container cont = containers[i];

            data.Prefix = "cont_" + i + "_";

            cont.transform.position.ToSaveData(data, "pos");
            cont.transform.rotation.eulerAngles.ToSaveData(data, "rot");

            string path = ResourcesPathHelper.GetContainerPath(cont);

            if (path != null)
                data.Add("path", path);

            cont.Save(data);
        }
    }

    public void Load(SaveData data)
    {
        ClearList();

        int count = int.Parse(data.Get("count"));

        for (int i = 0; i < count; i++)
        {
            data.Prefix = "cont_" + i + "_";

            string path = data.Get("path");

            if (path == null)
                continue;

            Vector3 pos = new Vector3().FromSaveData(data, "pos");
            Quaternion rot = Quaternion.Euler(new Vector3().FromSaveData(data, "rot"));

            GameObject prefab = Resources.Load(path) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning("Loading Pickables : Failed to load \"" + path + "\"");
                continue;
            }

            GameObject instance = Instantiate(prefab, pos, rot) as GameObject;

            instance.transform.parent = transform;

            Container cont = instance.GetComponent<Container>();

            if (cont != null)
                cont.Load(data);
        }
    }

    private void ClearList()
    {
        foreach (Container cont in containers)
        {
            Destroy(cont.gameObject);
        }

        containers.Clear();
    }
}