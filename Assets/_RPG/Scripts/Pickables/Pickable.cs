using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour, IPickable
{
    public string actionName = "Pickup";
    public string description = "This is a pickable object.";

    [SerializeField]
    private GameObject pickedItemPrefab = null;
    public GameObject PickedItemPrefab { get { return pickedItemPrefab; } }

    private object userData = null;
    public object UserData { get { return userData; } set { userData = value; } }

    private IItem prefabItem = null;

    private void Start()
    {
        if (pickedItemPrefab != null)
            prefabItem = pickedItemPrefab.GetComponent<IItem>();

        PickablesManager.RegisterPickable(this);
    }

    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        if (prefabItem != null)
            return description + " (W:" + prefabItem.Weigth + ")";

        return description;
    }

    public void Use(InteractManager user, UsableArgs args)
    {
        if (pickedItemPrefab != null)
        {
            IContainer container = user.gameObject.GetComponentInChildren<IContainer>();
            GameObject itemgo = GameObject.Instantiate(pickedItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            IItem item = itemgo.GetComponent<IItem>();
            if (item != null)
            {
                item.OnPickup(this);
                if (container != null && container.AddItem(item))
                    GameObject.Destroy(this.gameObject);
            }
        }
    }
}
