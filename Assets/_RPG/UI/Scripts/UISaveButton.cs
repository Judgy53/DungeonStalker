using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UISaveButton : MonoBehaviour, ISelectHandler
{
    public Text SaveId;
    public Text SaveName;

    public event EventHandler<EventArgs> OnClick;

    public void OnSelect(BaseEventData eventData)
    {
        if (!GetComponent<Button>().interactable)
            return;

        if (OnClick != null)
            OnClick(this, new EventArgs());
    }
}