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

    private SaveLoadState state;

    private void OnEnable()
    {
        state = GetComponentInParent<UISaveMenu>().State;

        ClearList();

        Dictionary<string, Save> saves = SaveManager.Instance.Saves;

        foreach (KeyValuePair<string, Save> kvp in saves)
        {
            string id = "";
            
            if (kvp.Value.autoSave)
                id = "Auto";
            else
                id = kvp.Key.Replace("save", "");

            string name = "Stage " + kvp.Value.Stage;

            CreateButton(id, name);
        }

        //create "New Save" Button
        if (state == SaveLoadState.Save)
            CreateButton("", "New Save");
    }

    private void CreateButton(string id, string name)
    {
        GameObject gao = Instantiate(buttonPrefab) as GameObject;
        UISaveButton button = gao.GetComponent<UISaveButton>();

        button.transform.SetParent(content, false);

        button.SaveId.text = id;
        button.SaveName.text = name;

        button.OnHover += SaveOnHover;
        button.OnClick += SaveOnClick;

        gao.transform.SetAsFirstSibling();
    }

    private void SaveOnHover(object sender, EventArgs e)
    {
        UISaveButton btn = (sender as MonoBehaviour).GetComponent<UISaveButton>();
        selectedSaveHandler.SetSave(btn.SaveId.text);
    }

    private void SaveOnClick(object sender, EventArgs e)
    {
        UISaveButton btn = (sender as MonoBehaviour).GetComponent<UISaveButton>();

        //no confirmation for now, will be needed later
        //SetButtonsState(false, btn);

        if(state == SaveLoadState.Save)
        {
            SaveManager.Instance.Save(false, btn.SaveId.text);
        }
        else if(state == SaveLoadState.Load)
        {
            SaveManager.Instance.Load(btn.SaveId.text);
        }

        GetComponentInParent<UISaveMenu>().gameObject.SetActive(false);
    }

    public void SetButtonsState(bool state, Button except = null)
    {
        Button[] buttons = content.GetComponentsInChildren<Button>();

        foreach(Button btn in buttons)
        {
            if (btn != null && btn != except)
                btn.interactable = state;
        }
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