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
    private string itemDescription = "Roasted chicken !";
    public string Description { get { return itemDescription; } }

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
    public string useDescription = "Will restore 10 life";
    
    public string GetActionName()
    {
        return actionName;
    }

    public string GetDescription()
    {
        return useDescription;
    }

    public void Use(InteractManager manager, UsableArgs args)
    {
        HealthManager playerHM = GetComponentInParent<HealthManager>();
        if (playerHM != null)
            playerHM.Heal(10.0f);

        manager.gameObject.GetComponentInChildren<PlayerContainer>().RemoveItem(this);
        Destroy(this.gameObject);
    }
}
