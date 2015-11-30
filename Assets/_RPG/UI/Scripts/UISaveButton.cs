using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UISaveButton : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private Text playerNameComp;
    
    [SerializeField]
    private Text stageComp;

    private string fileName = "";
    public string FileName { get { return fileName; } set { fileName = value; } }

    private Save data;
    public Save Data { get { return data; } set { data = value; } }

    public event EventHandler<EventArgs> OnClick;

    private void Start()
    {
        playerNameComp.text = data.PlayerName;
        stageComp.text = "Stage " + data.Stage;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!GetComponent<Button>().interactable)
            return;

        if (OnClick != null)
            OnClick(this, new EventArgs());
    }

    public string GetSaveId()
    {
        return fileName.Replace("save", "");
    }
}