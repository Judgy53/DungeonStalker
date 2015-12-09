using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIObjectButtonsManager : MonoBehaviour
{
    public UIInventoryList inventoryListManager = null;

    public bool autoAddListeners = true;

    public Button useButton = null;
    public Button mainHandEquipButton = null;
    public Button offHandEquipButton = null;
    public Button dropButton = null;

    private IItem lastSelectedItem = null;

    private void Start()
    {
        if (inventoryListManager == null)
        {
            Debug.LogError("No inventoryManager defined on " + this.name);
            enabled = false;
            return;
        }

        inventoryListManager.OnItemFocusChange += OnItemFocusChangeCallback;

        if (autoAddListeners)
        {
            if (useButton != null)
                useButton.onClick.AddListener(OnUseCallback);
            if (mainHandEquipButton != null)
                mainHandEquipButton.onClick.AddListener(OnMainHandEquipCallback);
            if (offHandEquipButton != null)
                offHandEquipButton.onClick.AddListener(OnOffHandEquipCallback);
            if (dropButton != null)
                dropButton.onClick.AddListener(OnDropCallback);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (useButton != null)
            {
                UnityEngine.EventSystems.PointerEventData pointer = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                UnityEngine.EventSystems.ExecuteEvents.Execute(useButton.gameObject, pointer, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (dropButton != null)
            {
                UnityEngine.EventSystems.PointerEventData pointer = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                UnityEngine.EventSystems.ExecuteEvents.Execute(dropButton.gameObject, pointer, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
            }
        }
    }

    private void OnUseCallback()
    {
        if (lastSelectedItem is IUsable)
        {
            (lastSelectedItem as IUsable).Use((lastSelectedItem as Behaviour).GetComponentInParent<InteractManager>());

            inventoryListManager.Populate(GetComponentInParent<UIInventoryMenu>().target);
            inventoryListManager.SelectFirst();
        }
    }

    private void OnDropCallback()
    {
        if (lastSelectedItem.CanDrop)
        {
            GetComponentInParent<UIInventoryMenu>().target.DropItem(lastSelectedItem);

            inventoryListManager.Populate(GetComponentInParent<UIInventoryMenu>().target);
            inventoryListManager.SelectFirst();
        }
    }

    private void OnMainHandEquipCallback()
    {
        if (lastSelectedItem is ItemWeapon)
            (lastSelectedItem as ItemWeapon).Use((lastSelectedItem as Behaviour).GetComponentInParent<InteractManager>(), new EquipWeaponArgs(EquipWeaponArgs.Hand.MainHand));

        inventoryListManager.Populate(GetComponentInParent<UIInventoryMenu>().target);
        inventoryListManager.SelectFirst();
    }

    private void OnOffHandEquipCallback()
    {
        if (lastSelectedItem is ItemWeapon)
            (lastSelectedItem as ItemWeapon).Use((lastSelectedItem as Behaviour).GetComponentInParent<InteractManager>(), new EquipWeaponArgs(EquipWeaponArgs.Hand.OffHand));

        inventoryListManager.Populate(GetComponentInParent<UIInventoryMenu>().target);
        inventoryListManager.SelectFirst();
    }

    private void OnItemFocusChangeCallback(object sender, ItemFocusChangeArgs args)
    {
        if (args.newItem == null)
        {
            ChangeButtonState(useButton, false);
            ChangeButtonState(dropButton, false);
            ChangeButtonState(mainHandEquipButton, false);
            ChangeButtonState(offHandEquipButton, false);

            return;
        }
        else
        {
            ChangeButtonState(useButton, true);
            ChangeButtonState(dropButton, true);
            ChangeButtonState(mainHandEquipButton, true);
            ChangeButtonState(offHandEquipButton, true);
        }

        if (args.newItem is ItemWeapon)
        {
            ChangeButtonState(useButton, false);

            WeaponManager wm = null;
            switch ((args.newItem as ItemWeapon).Restriction)
            {
                case WeaponRestriction.Both:
                    ChangeButtonState(mainHandEquipButton, true);
                    wm = GetComponentInParent<UIInventoryMenu>().target.GetComponentInParent<WeaponManager>();
                    if (wm != null && (wm.MainHandWeapon == null || wm.MainHandWeapon.WeaponHand == WeaponHand.OneHanded))
                        ChangeButtonState(offHandEquipButton, true);
                    else
                        ChangeButtonState(offHandEquipButton, false);
                    break;
                case WeaponRestriction.MainHand:
                    ChangeButtonState(mainHandEquipButton, true);
                    ChangeButtonState(offHandEquipButton, false);
                    break;
                case WeaponRestriction.OffHand:
                    ChangeButtonState(mainHandEquipButton, false);
                    wm = GetComponentInParent<UIInventoryMenu>().target.GetComponentInParent<WeaponManager>();
                    if (wm != null && (wm.MainHandWeapon == null || wm.MainHandWeapon.WeaponHand == WeaponHand.OneHanded))
                        ChangeButtonState(offHandEquipButton, true);
                    else
                        ChangeButtonState(offHandEquipButton, false);
                    break;
            }
        }
        else
        {
            ChangeButtonState(mainHandEquipButton, false);
            ChangeButtonState(offHandEquipButton, false);

            if (useButton != null)
            {
                if (args.newItem is IUsable)
                    ChangeButtonState(useButton, true);
                else
                    ChangeButtonState(useButton, false);
            }

            if (dropButton != null)
                ChangeButtonState(dropButton, args.newItem.CanDrop);
        }

        lastSelectedItem = args.newItem;
    }

    private void ChangeButtonState(Button button, bool state)
    {
        if (button != null)
        {
            button.enabled = state;
            button.GetComponent<Image>().enabled = state;
            button.GetComponentInChildren<Text>().enabled = state;
        }
    }
}
