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
            CreateButton(kvp.Key, kvp.Value);


        selectedSaveHandler.SetSave("");
    }

    private void CreateButton(string saveFile, Save sav)
    {
        GameObject gao = Instantiate(buttonPrefab) as GameObject;
        UISaveButton button = gao.GetComponent<UISaveButton>();

        button.transform.SetParent(content, false);

        button.FileName = saveFile;
        button.Data = sav;
        
        button.OnClick += SaveOnClick;

        gao.transform.SetAsFirstSibling();
    }

    private void SaveOnClick(object sender, EventArgs e)
    {
        UISaveButton btn = (sender as MonoBehaviour).GetComponent<UISaveButton>();
        selectedSaveHandler.SetSave(btn.GetSaveId());

        GetComponentInParent<UIPanelLoad>().SaveId = btn.GetSaveId();

        UIUtils.UpdateScroller(content, GetComponent<RectTransform>(), btn.GetComponent<RectTransform>());
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
}