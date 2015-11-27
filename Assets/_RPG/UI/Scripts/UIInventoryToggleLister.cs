using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIInventoryToggleLister : MonoBehaviour
{
    public Toggle togglePrefab = null;
    private List<Toggle> toggleList = new List<Toggle>();

    public event EventHandler<ItemFocusChangeArgs> OnItemFocusChange;

    public RectTransform content = null;
    public RectTransform background = null;

    public float spacing = 5.0f;
    public string weightPrefix = "Weight : ";

    [SerializeField]
    private ToggleGroup toggleGroup = null;

    [SerializeField]
    private Text weightText = null;

    private IItem[] items;

    private IItem selectedItem = null;
    public IItem SelectedItem
    {
        get { return selectedItem; }
        private set 
        {
            if (OnItemFocusChange != null)
                OnItemFocusChange(this, new ItemFocusChangeArgs(value));

            selectedItem = value;
        }
    }

    private IContainer target = null;
    public IContainer Target { get { return target; } }

    [SerializeField]
    private Animator weightAnimator = null;

    public void Start()
    {
        if (toggleGroup == null || togglePrefab == null || content == null || background == null)
        {
            Debug.LogError(this.name + " is not configured properly. Disabling ...");
            enabled = false;
        }
    }

    public void Populate(IContainer container)
    {
        if (toggleList.Count != 0)
            Clear();

        target = container;

        items = container.Items;

        float elementHeight = togglePrefab.GetComponent<RectTransform>().rect.height;

        content.sizeDelta = new Vector2(content.sizeDelta.x, elementHeight);
        background.sizeDelta = new Vector2(background.sizeDelta.x, elementHeight);

        int i = 0;
        foreach (IItem item in items)
        {
            Toggle toggle = GameObject.Instantiate(togglePrefab, Vector3.zero, Quaternion.identity) as Toggle;
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener(OnToggleChangeValue);

            GameObject toggleGo = toggle.gameObject;

            Text text = toggleGo.GetComponentInChildren<Text>();

            toggleGo.transform.SetParent(content, false);

            toggleGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, (elementHeight * -i) + (-i * spacing));

            if (text != null)
                text.text = item.Name + " (W:" + item.Weigth + ")";

            content.sizeDelta = new Vector2(content.sizeDelta.x, elementHeight * (i + 1) + (i * spacing));
            background.sizeDelta = new Vector2(background.sizeDelta.x, elementHeight * (i + 1) + (i * spacing));

            toggleList.Add(toggle);
            toggleGroup.RegisterToggle(toggle);

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

        toggleGroup.SetAllTogglesOff();

        if (items.Length != 0)
        {
            SelectedItem = items[0];
            toggleList[0].isOn = true;
        }
        else
            SelectedItem = null;
    }

    public void Clear()
    {
        target = null;

        foreach (var t in toggleList)
        {
            toggleGroup.UnregisterToggle(t);
            GameObject.Destroy(t.gameObject);
        }

        items = new IItem[0];

        toggleList.Clear();
    }

    public void PlayWeightAnim()
    {
        if (weightAnimator != null)
            weightAnimator.SetTrigger("Trigger");
    }

    private void OnToggleChangeValue(bool value)
    {
        List<Toggle> activeToggles = new List<Toggle>(toggleGroup.ActiveToggles());
        if (activeToggles.Count > 1)
        {
            Debug.LogError("Error on selected toggles");
            return;
        }

        if (activeToggles.Count != 0)
        {
            int index = toggleList.IndexOf(activeToggles[0]);
            if (index == -1)
            {
                Debug.LogWarning("Configuration error on item/toggles relation in " + this.name);
                SelectedItem = null;
                return;
            }
            SelectedItem = items[index];
        }
        else
            SelectedItem = null;
    }
}
