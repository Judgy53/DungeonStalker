using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItemDescription : MonoBehaviour
{
    public UIInventoryList inventoryListManager = null;

    public Text objectTitle = null;
    public Text objectDescription = null;
    public Image objectImage = null;

    private Sprite defaultSprite = null;

    private void Start()
    {
        if (inventoryListManager == null)
        {
            Debug.LogError("No inventoryManager defined on " + this.name);
            enabled = false;
        }

        inventoryListManager.OnItemFocusChange += OnItemFocusChangeCallback;
        
        if (objectImage != null)
            defaultSprite = objectImage.sprite;
    }

    private void OnItemFocusChangeCallback(object sender, ItemFocusChangeArgs args)
    {
        if (args.newItem == null)
        {
            if (objectTitle != null)
                objectTitle.enabled = false;
            if (objectDescription != null)
                objectDescription.enabled = false;
            if (objectImage != null)
                objectImage.enabled = false;

            return;
        }
        else
        {
            if (objectTitle != null)
                objectTitle.enabled = true;
            if (objectDescription != null)
                objectDescription.enabled = true;
            if (objectImage != null)
                objectImage.enabled = true;
        }

        if (objectTitle != null)
            objectTitle.text = args.newItem.Name;
        
        if (objectDescription != null)
        {
            objectDescription.text = args.newItem.Description;
            if (args.newItem is IUsable)
                objectDescription.text += "\nUse : " + (args.newItem as IUsable).GetDescription();
        }

        if (objectImage != null)
        {
            if (args.newItem.Image != null)
                objectImage.sprite = args.newItem.Image;
            else
                objectImage.sprite = defaultSprite;
        }
    }
}
