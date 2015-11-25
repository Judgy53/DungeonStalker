using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class UISaveList : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private UICurrentSave selectedSaveHandler;

    private UIScrollRect scroller;

    private void OnEnable()
    {
        if (scroller == null)
            scroller = GetComponent<UIScrollRect>();

        ClearList();

        Dictionary<string, Save> saves = SaveManager.Instance.Saves;

        foreach (KeyValuePair<string, Save> kvp in saves)
        {
            string id = kvp.Key.Replace("save", "");

            string name = "Stage " + kvp.Value.Stage;

            CreateButton(id, name);
        }

        selectedSaveHandler.SetSave("");
    }

    private void CreateButton(string id, string name)
    {
        GameObject gao = Instantiate(buttonPrefab) as GameObject;
        UISaveButton button = gao.GetComponent<UISaveButton>();

        button.transform.SetParent(content, false);

        button.SaveId.text = id;
        button.SaveName.text = name;
        
        button.OnClick += SaveOnClick;

        gao.transform.SetAsFirstSibling();
    }

    private void SaveOnClick(object sender, EventArgs e)
    {
        UISaveButton btn = (sender as MonoBehaviour).GetComponent<UISaveButton>();
        selectedSaveHandler.SetSave(btn.SaveId.text);

        GetComponentInParent<UIPanelLoad>().SaveId = btn.SaveId.text;

        UpdateScroller(btn);
    }

    private void OnDisable()
    {
        ClearList();
    }

    private void ClearList()
    {
        foreach (Transform tr in content)
            Destroy(tr.gameObject);
    }

    private void UpdateScroller(UISaveButton btn)
    {
        float scrolled = content.anchoredPosition.y;
        float holderHeight = GetComponent<RectTransform>().sizeDelta.y;

        float btnHeight = btn.GetComponent<RectTransform>().sizeDelta.y;

        float pos = btn.transform.GetSiblingIndex() * btnHeight;

        if(pos < scrolled + btnHeight) // if selected button overflow on top
        {
            content.anchoredPosition = new Vector2(0f, pos);
        }
        else if (pos + btnHeight > scrolled + holderHeight) // if selected button overflow on bottom
        {
            content.anchoredPosition = new Vector2(0f, pos - btnHeight * (holderHeight / btnHeight - 1));
        }
    }
}