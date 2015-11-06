using UnityEngine;
using System.Collections;

public class PickablesSave : MonoBehaviour, ISavable
{

    public void Save(SaveData data)
    {
        Pickable[] childs = GetComponentsInChildren<Pickable>();

        int count = 0;

        foreach (Pickable pick in childs)
        {
            string prefix = "pick_" + count++ + "_";

            pick.transform.position.ToSaveData(data, prefix + "pos");
            pick.transform.rotation.eulerAngles.ToSaveData(data, prefix + "rot");

            data.Add(prefix + "path", ResourcesPathHelper.GetPickablePath(pick.name));
        }
    }

    public void Load(SaveData data)
    {
        ClearList();

        int count = 0;

        while (true)
        {
            string prefix = "pick_" + count++ + "_";

            string path = data.Get(prefix + "path");

            if (path == null)
                break;

            Vector3 pos = new Vector3().FromSaveData(data, prefix + "pos");
            Quaternion rot = Quaternion.Euler(new Vector3().FromSaveData(data, prefix + "rot"));

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
        Pickable[] childs = GetComponentsInChildren<Pickable>();

        foreach (Pickable pick in childs)
        {
            Destroy(pick.gameObject);
        }
    }
}
