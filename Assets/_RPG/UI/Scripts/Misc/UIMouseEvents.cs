using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class UIMouseEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class UIMouseEvent : UnityEvent<UIMouseEvents> { }
    
    public UIMouseEvent onMouseEnter;
    public UIMouseEvent onMouseExit;

    public UIMouseEvent onMouseLeftUp;
    public UIMouseEvent onMouseLeftDown;

    public UIMouseEvent onMouseRightUp;
    public UIMouseEvent onMouseRightDown;

    private bool isOver = false;

    private void Update()
    {
        if (isOver)
        {
            if (Input.GetMouseButtonDown(0))
                onMouseLeftDown.Invoke(this);
            if (Input.GetMouseButtonDown(1))
                onMouseRightDown.Invoke(this);
            if (Input.GetMouseButtonUp(0))
                onMouseLeftUp.Invoke(this);
            if (Input.GetMouseButtonUp(1))
                onMouseRightUp.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        onMouseEnter.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        onMouseExit.Invoke(this);
    }
}