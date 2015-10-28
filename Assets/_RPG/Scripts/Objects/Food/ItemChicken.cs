using UnityEngine;
using System.Collections;

public class ItemChicken : MonoBehaviour, IItem, IUsable
{
    [SerializeField]
    private Sprite image = null;
    public Sprite Image { get { return image; } }

    [SerializeField]
    private string objName = "Chicken";
    public string Name { get { return objName; } }

    [SerializeField]
    private uint weigth = 1u;
    public uint Weigth { get { return weigth; } }

    [SerializeField]
    private bool canDrop = true;
    public bool CanDrop { get { return canDrop; } set { canDrop = value; } }

    [SerializeField]
    private GameObject dropPrefab = null;
    public GameObject DropPrefab { get { return dropPrefab; } }

    public string actionName = "Eat";
    public string description = "Delicious, delicious chicken ...";
    
    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        return description;
    }

    public void Use(InteractManager manager)
    {
        IDamageable playerHM = GetComponentInParent<IDamageable>();
        if (playerHM != null)
            playerHM.AddDamage(-10.0f);

        Destroy(this.gameObject);
    }
}
