using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIInventoryList : MonoBehaviour
{
    public event EventHandler<ItemFocusChangeArgs> OnItemFocusChange;

    [SerializeField]
    private GameObject buttonTemplatePrefab = null;

    [SerializeField]
    private RectTransform content = null;

    [SerializeField]
    private string weightPrefix = "Weight : ";

    [SerializeField]
    private Text weightText = null;

    private List<GameObject> instantiatedList = new List<GameObject>();

    private UIInventoryMenu manager = null;

    private GameObject lastSelected = null;

    private IItem[] items = null;

    private IContainer target = null;
    public IContainer Target { get { return target; } }

    private ItemType currentFilter = ItemType.All;
    public ItemType CurrentFilter { get { return currentFilter; } set { currentFilter = value; } }

    private UIButtonsSoundHandler soundHandler;

    [SerializeField]
    private AudioClip equipClip;

    [SerializeField]
    private AudioClip dropClip;

    private void Start()
    {
        soundHandler = GetComponentInParent<UIButtonsSoundHandler>();

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

        manager = GetComponentInParent<UIInventoryMenu>();
        if (manager != null)
            manager.OnInventoryMenuStateChange += OnInventoryMenuStateChangeCallback;
    }

    private void Update()
    {
        GameObject currentSelected = manager.eventSystem.currentSelectedGameObject;
        if (currentSelected != lastSelected)
        {
            if (instantiatedList.IndexOf(currentSelected) != -1)
            {
                lastSelected = currentSelected;

                if (OnItemFocusChange != null)
                {
                    IItem item = currentSelected.GetComponent<UIInventoryButton>().Item;
                    OnItemFocusChange(this, new ItemFocusChangeArgs(item));
                }

                UIUtils.UpdateScroller(content, GetComponent<RectTransform>(), currentSelected.GetComponent<RectTransform>());
            }
        }

        if(Input.GetKeyDown(KeyCode.E) && instantiatedList.IndexOf(currentSelected) != -1)
        {
            Button_onLeftClick(currentSelected.GetComponent<UIMouseEvents>());
        }

        if (Input.GetKeyDown(KeyCode.R) && instantiatedList.IndexOf(currentSelected) != -1)
        {
            Button_onDrop(currentSelected.GetComponent<UIInventoryButton>());
        }
    }

    private void OnInventoryMenuStateChangeCallback(object sender, UIMenuStateChangeArgs e)
    {
        IContainer container = (sender as UIInventoryMenu).target;

        if (e.newState == UIMenuState.Shown && container != null)
            Populate(container);
        else
            ClearList();
    }

    public void Populate(IContainer container)
    {
        int selectPos = 0;
        if (lastSelected != null)
            selectPos = lastSelected.transform.GetSiblingIndex();

        if (instantiatedList.Count != 0)
            ClearList();

        target = container;
        items = container.Items;

        foreach(IItem item in items)
        {
            if (currentFilter != ItemType.All && item.Type != currentFilter)
                continue;

            GameObject buttonGao = GameObject.Instantiate(buttonTemplatePrefab, Vector3.zero, Quaternion.identity) as GameObject;

            buttonGao.transform.SetParent(content, false);

            Text text = buttonGao.GetComponentInChildren<Text>();

            text.color = item.Quality.ToColor();
            text.text = item.Name;

            instantiatedList.Add(buttonGao);

            buttonGao.GetComponent<UIInventoryButton>().Item = item;

            UIMouseEvents events = buttonGao.GetComponent<UIMouseEvents>();
            events.onMouseEnter.AddListener(Button_onMouseEnter);
            events.onMouseLeftUp.AddListener(Button_onLeftClick);
            events.onMouseRightUp.AddListener(Button_onRightClick);
        }

        selectPos = Mathf.Clamp(selectPos - 1, 0, instantiatedList.Count - 1);

        if (instantiatedList.Count > 0)
            instantiatedList[selectPos].GetComponent<Selectable>().Select();
        else
        {
            if (OnItemFocusChange != null)
                OnItemFocusChange(this, new ItemFocusChangeArgs(null));
        }

        UpdateWeight();
    }

    private void UpdateWeight()
    {
        if (weightText != null)
        {
            weightText.text = weightPrefix + target.CurrentWeight + "/" + target.MaxWeight;

            if (target.CurrentWeight <= target.MaxWeight)
                weightText.color = Color.white;
            else
                weightText.color = Color.red;
        }
    }
    
    public void ClearList()
    {
        target = null;

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

    private void Button_onMouseEnter(UIMouseEvents btn)
    {
        btn.GetComponent<Selectable>().Select();

        soundHandler.onBtnHover();
    }

    private void Button_onLeftClick(UIMouseEvents btn)
    {
        IItem item = btn.GetComponent<UIInventoryButton>().Item;

        if (item is ItemWeapon && CanEquip(item as ItemWeapon, WeaponRestriction.MainHand))
            (item as ItemWeapon).Use((item as Behaviour).GetComponentInParent<InteractManager>(), new EquipWeaponArgs(EquipWeaponArgs.Hand.MainHand));
        else if (item is IUsable)
            (item as IUsable).Use((item as Behaviour).GetComponentInParent<InteractManager>());

        Populate(target);

        AudioManager.PlaySfx(equipClip, Camera.main.transform);
    }

    private void Button_onRightClick(UIMouseEvents btn)
    {
        IItem item = btn.GetComponent<UIInventoryButton>().Item;

        if (item is ItemWeapon && CanEquip(item as ItemWeapon, WeaponRestriction.OffHand))
            (item as ItemWeapon).Use((item as Behaviour).GetComponentInParent<InteractManager>(), new EquipWeaponArgs(EquipWeaponArgs.Hand.OffHand));

        Populate(target);
    }

    private void Button_onDrop(UIInventoryButton btn)
    {
        GetComponentInParent<UIInventoryMenu>().target.DropItem(btn.Item);

        Populate(target);

        AudioManager.PlaySfx(dropClip, Camera.main.transform);
    }

    private bool CanEquip(ItemWeapon weapon, WeaponRestriction slot)
    {
        WeaponManager wm = GetComponentInParent<UIInventoryMenu>().target.GetComponentInParent<WeaponManager>();

        switch (weapon.Restriction)
        {
            case WeaponRestriction.Both:
            case WeaponRestriction.OffHand:
                if (wm != null && (wm.MainHandWeapon == null || wm.MainHandWeapon.WeaponHand == WeaponHand.OneHanded))
                    return true;
                else
                    return slot == WeaponRestriction.MainHand;
            case WeaponRestriction.MainHand:
                return slot == WeaponRestriction.MainHand;
        }

        return false;
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