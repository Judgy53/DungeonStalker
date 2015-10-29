using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIObjectButtonsManager : MonoBehaviour
{
    public UIInventoryList inventoryListManager = null;

    public bool autoAddListeners = true;

    public Button useButton = null;
    public Button dropButton = null;

    private IItem lastSelectedItem = null;

    private void Start()
    {
        if (inventoryListManager == null)
        {
            Debug.LogError("No inventoryManager defined on " + this.name);
            enabled = false;
        }

        inventoryListManager.OnItemFocusChange += OnItemFocusChangeCallback;

        if (autoAddListeners)
        {
            if (useButton != null)
                useButton.onClick.AddListener(OnUseCallback);
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
            (lastSelectedItem as IUsable).Use((lastSelectedItem as Behaviour).GetComponentInParent<InteractManager>());

        inventoryListManager.Populate(GetComponentInParent<UIItemPauseMenu>().target);
        inventoryListManager.SelectFirst();
    }

    private void OnDropCallback()
    {
        if (lastSelectedItem.CanDrop)
            GetComponentInParent<UIItemPauseMenu>().target.DropItem(lastSelectedItem);

        inventoryListManager.Populate(GetComponentInParent<UIItemPauseMenu>().target);
        inventoryListManager.SelectFirst();
    }

    private void OnItemFocusChangeCallback(object sender, ItemFocusChangeArgs args)
    {
        if (args.newItem == null)
        {
            if (useButton != null)
            {
                useButton.enabled = false;
                useButton.GetComponent<Image>().enabled = false;
                useButton.GetComponentInChildren<Text>().enabled = false;
            }

            if (dropButton != null)
            {
                dropButton.enabled = false;
                dropButton.GetComponent<Image>().enabled = false;
                dropButton.GetComponentInChildren<Text>().enabled = false;
            }

            return;
        }
        else
        {
            if (useButton != null)
            {
                useButton.enabled = true;
                useButton.GetComponent<Image>().enabled = true;
                useButton.GetComponentInChildren<Text>().enabled = true;
            }

            if (dropButton != null)
            {
                dropButton.enabled = true;
                dropButton.GetComponent<Image>().enabled = true;
                dropButton.GetComponentInChildren<Text>().enabled = true;
            }
        }

        if (useButton != null)
        {
            if (args.newItem is IUsable)
                useButton.gameObject.SetActive(true);
            else
                useButton.gameObject.SetActive(false);
        }

        if (dropButton != null)
            dropButton.gameObject.SetActive(args.newItem.CanDrop);

        lastSelectedItem = args.newItem;
    }
}
