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

    [SerializeField]
    private Text helpText = null;

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
        if (helpText != null)
            UpdateHelpText(args.newItem);

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
            if (args.newItem is ItemWeapon)
            {
                ItemWeapon weapItem = args.newItem as ItemWeapon;

                IWeapon weapon = weapItem.weaponPrefab.GetComponent<IWeapon>();
                if (weapon != null)
                {
                    if (weapon.GearStats != 0)
                    {
                        objectDescription.text += "<color=green>\nStats : ";
                        if (weapon.GearStats.Strength != 0)
                            objectDescription.text += weapon.GearStats.Strength + " STR | ";
                        if (weapon.GearStats.Stamina != 0)
                            objectDescription.text += weapon.GearStats.Stamina + " STA | ";
                        if (weapon.GearStats.Defense != 0)
                            objectDescription.text += weapon.GearStats.Defense + " DEF | ";
                        if (weapon.GearStats.Energy != 0)
                            objectDescription.text += weapon.GearStats.Energy + " ENG";
                        objectDescription.text += "</color>";
                    }

                    objectDescription.text += weapon.GetInventoryDescription();
                }
            }
        }

        if (objectImage != null)
        {
            if (args.newItem.Image != null)
                objectImage.sprite = args.newItem.Image;
            else
                objectImage.sprite = defaultSprite;
        }
    }

    private void UpdateHelpText(IItem item)
    {
        string text = "";

        if (item != null)
        {
            if (item is ItemArmor)
                text = "E - Equip";
            if (item is ItemWeapon)
                text = "E - Equip Right Hand    Right Click - Equip Left Hand";
            else if (item is IUsable)
                text = "E - Use";
            
            text += "    R - Drop";
        }

        helpText.text = text;
    }
}
