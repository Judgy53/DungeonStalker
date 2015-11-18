using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UISaveButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public Text SaveId;
    public Text SaveName;

    public event EventHandler<EventArgs> OnHover;
    public event EventHandler<EventArgs> OnClick;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Button>().interactable)
            return;

        if (OnHover != null)
            OnHover(this, new EventArgs());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
            OnClick(this, new EventArgs());
    }
}