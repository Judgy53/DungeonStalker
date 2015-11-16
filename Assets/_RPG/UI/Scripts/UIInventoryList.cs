using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIInventoryList : MonoBehaviour
{
    public event EventHandler<ItemFocusChangeArgs> OnItemFocusChange;

    public GameObject buttonTemplatePrefab = null;

    public RectTransform content = null;
    public RectTransform background = null;

    public string weightPrefix = "Weight : ";
    public Text weightText = null;

    public float spacing = 5.0f;

    private List<GameObject> instantiatedList = new List<GameObject>();

    private UIItemPauseMenu manager = null;

    private GameObject lastSelected = null;

    private IItem[] items = null;

    private void Start()
    {
        if (buttonTemplatePrefab == null)
        {
            Debug.LogError("No button template defined on " + this.name);
            enabled = false;
        }

        if (content == null)
        {
            Debug.LogError("No content defined on " + this.name);
            enabled = false;
        }

        manager = GetComponentInParent<UIItemPauseMenu>();
        if (manager != null)
            manager.OnItemPauseMenuStateChange += OnItemPauseMenuStateChangeCallback;
    }

    private void Update()
    {
        GameObject currentSelected = manager.eventSystem.currentSelectedGameObject;
        if (currentSelected != lastSelected)
        {
            int index = -1;
            if ((index = instantiatedList.IndexOf(currentSelected)) != -1)
            {
                lastSelected = currentSelected;

                if (OnItemFocusChange != null)
                    OnItemFocusChange(this, new ItemFocusChangeArgs(items[index]));
            }
        }
    }

    private void OnItemPauseMenuStateChangeCallback(object sender, UIMenuStateChangeArgs e)
    {
        IContainer container = (sender as UIItemPauseMenu).target;

        if (e.newState == UIMenuState.Shown && container != null)
            Populate(container);
        else
            Clear();
    }

    public void Populate(IContainer container)
    {
        if (instantiatedList.Count != 0)
            Clear();

        items = container.Items;

        float buttonHeight = buttonTemplatePrefab.GetComponent<RectTransform>().rect.height;

        content.sizeDelta = new Vector2(content.sizeDelta.x, buttonHeight);
        background.sizeDelta = new Vector2(background.sizeDelta.x, buttonHeight);

        int i = 0;
        foreach (IItem item in items)
        {
            GameObject button = GameObject.Instantiate(buttonTemplatePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Text text = button.GetComponentInChildren<Text>();

            button.transform.SetParent(content, false);
            
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, (buttonHeight * -i) + (-i * spacing));
            
            if (text != null)
                text.text = item.Name + " (W:" + item.Weigth + ")";

            content.sizeDelta = new Vector2(content.sizeDelta.x, buttonHeight * (i + 1) + (i * spacing));
            background.sizeDelta = new Vector2(background.sizeDelta.x, buttonHeight * (i + 1) + (i * spacing));

            instantiatedList.Add(button);

            ++i;
        }

        if (weightText != null)
        {
            weightText.text = weightPrefix + container.CurrentWeight + "/" + container.MaxWeight;

            if (container.CurrentWeight <= container.MaxWeight)
                weightText.color = Color.white;
            else
                weightText.color = Color.red;
        }

        if (instantiatedList.Count > 0)
            instantiatedList[0].GetComponent<Selectable>().Select();
        else
        {
            if (OnItemFocusChange != null)
                OnItemFocusChange(this, new ItemFocusChangeArgs(null));
        }
    }

    public void Clear()
    {
        foreach (var button in instantiatedList)
            GameObject.Destroy(button);

        instantiatedList.Clear();
        items = new IItem[0];
    }

    public void SelectFirst()
    {
        if (instantiatedList.Count != 0)
            manager.eventSystem.SetSelectedGameObject(instantiatedList[0]);
        else
        {
            if (OnItemFocusChange != null)
                OnItemFocusChange(this, new ItemFocusChangeArgs(null));
        }
    }
}


public class ItemFocusChangeArgs : EventArgs
{
    public IItem newItem;

    public ItemFocusChangeArgs(IItem item)
    {
        newItem = item;
    }
}