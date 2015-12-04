using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour, IContainer, IUsable
{
    [SerializeField]
    private float maxWeight = 100.0f;
    public float MaxWeight { get { return maxWeight; } set { maxWeight = value; } }

    private float currentWeight = 0.0f;
    public float CurrentWeight
    {
        get { return currentWeight; }
        private set { currentWeight = value; }
    }

    public IItem[] Items
    {
        get
        {
            List<IItem> results = new List<IItem>(GetComponentsInChildren<IItem>());
            //Somehow, GetComponentsInChildren returns destroyed components on the same frame ...
            results.RemoveAll(x => !(x as Behaviour).enabled);
            return results.ToArray();
        }
    }

    public string actionName = "Open";
    public string actionDescription = "";

    private UITransfer transferUI = null;


    /// <summary>
    /// DEBUG !
    /// </summary>
    public GameObject[] items = null;

    private void Start()
    {
        transferUI = gameObject.GetComponentInChildrenWithTag<UITransfer>("UI");
        if (transferUI == null)
            Debug.LogWarning("No TransferUI found !");

        if (items != null && Items.Length == 0) // don't fill with debug items if data already loaded
        {
            foreach (var i in items)
                AddItem((GameObject.Instantiate(i, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<IItem>());
        }

        ContainersManager.RegisterContainer(this);
    }

    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        if (Items.Length > 0)
            return actionDescription;
        else
            return "Empty.";
    }

    public void Use(InteractManager user, UsableArgs args = null)
    {
        if (transferUI != null)
            transferUI.OpenUI(user.GetComponentInChildren<IContainer>(), this);

    }

    public bool AddItem(IItem item)
    {
        if (currentWeight + item.Weigth > maxWeight)
            return false;

        (item as Behaviour).transform.SetParent(gameObject.transform, false);
        CurrentWeight += item.Weigth;

        return true;
    }

    public bool RemoveItem(IItem item)
    {
        Behaviour itemBehaviour = (item as Behaviour);
        if (itemBehaviour.transform.IsChildOf(transform))
        {
            itemBehaviour.transform.SetParent(null, false);
            CurrentWeight -= item.Weigth;
            return true;
        }

        return false;
    }

    private void ClearInventory()
    {
        IItem[] items = Items;

        foreach (IItem item in items)
        {
            RemoveItem(item);
            Destroy((item as Behaviour).gameObject);
        }
    }

    public void Save(SaveData data)
    {
        IItem[] items = Items;

        int count = items.Length;

        data.Add("count", count);

        for (int i = 0; i < count; i++)
        {
            IItem item = items[i];

            data.Add("Item_" + i, ResourcesPathHelper.GetItemPath(item));
        }
    }

    public void Load(SaveData data)
    {
        ClearInventory();

        int count = int.Parse(data.Get("count"));

        for (int i = 0; i < count; i++)
        {
            string path = data.Get("Item_" + i);
            if (path == null)
                break;

            GameObject prefab = Resources.Load(path) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning("Loading Container : Failed to load \"" + path + "\"");
                continue;
            }

            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

            AddItem(instance.GetComponent<IItem>());
        }
    }
}