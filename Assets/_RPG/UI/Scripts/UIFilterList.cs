using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIFilterList : MonoBehaviour
{
    [SerializeField]
    private UIInventoryList inventory;

    [SerializeField]
    private GameObject content;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private ItemType[] filters;

    private List<GameObject> buttons = new List<GameObject>();

    private void Start()
    {
        foreach(ItemType type in filters)
        {
            GameObject buttonGao = Instantiate<GameObject>(buttonPrefab);
            Text text = buttonGao.GetComponentInChildren<Text>();

            text.text = type.ToString().ToUpper();
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Bold;

            buttonGao.transform.SetParent(content.transform, false);

            UIMouseEvents events = buttonGao.GetComponent<UIMouseEvents>();
            events.onMouseLeftUp.AddListener(Button_onClick);

            buttons.Add(buttonGao);
        }
    }

    private void OnEnable()
    {
        inventory.CurrentFilter = ItemType.All;
    }

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E))
        {
            if (selected != null && selected.transform.IsChildOf(content.transform))
            {
                Button_onClick(selected.GetComponent<UIMouseEvents>());

                //SelectButton(selected);
            }
        }
    }

    private void Button_onClick(UIMouseEvents btn)
    {
        inventory.CurrentFilter = filters[btn.transform.GetSiblingIndex()];

        inventory.Populate(inventory.Target);
    }

    private void SelectButton(GameObject select)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);

        foreach (GameObject btn in buttons)
        {
            if (btn == select)
                ExecuteEvents.Execute(select, pointer, ExecuteEvents.pointerEnterHandler);
            else
                ExecuteEvents.Execute(select, pointer, ExecuteEvents.pointerExitHandler);
        }
    }
}