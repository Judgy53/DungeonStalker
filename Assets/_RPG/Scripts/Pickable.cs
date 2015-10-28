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

    private IItem prefabItem = null;

    private void Start()
    {
        if (pickedItemPrefab != null)
            prefabItem = pickedItemPrefab.GetComponent<IItem>();
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

    public void Use(InteractManager user)
    {
        if (pickedItemPrefab != null)
        {
            IContainer container = user.gameObject.GetComponentInChildren<IContainer>();
            GameObject itemgo = GameObject.Instantiate(pickedItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            IItem item = itemgo.GetComponent<IItem>();
            if (item != null && container != null)
            {
                if (container.AddItem(item))
                    GameObject.Destroy(this.gameObject);
            }
        }
    }
}
