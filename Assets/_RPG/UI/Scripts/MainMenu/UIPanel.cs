using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour
{
    private RectTransform rect;

    private bool closed = true;

    [SerializeField]
    private float openedPos = 150f;

    [SerializeField]
    private float transitionSpeed = 25f;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(500f, 0f);
    }

    private void OnDisable()
    {
        EventSystem eventSys = EventSystem.current;
        if (eventSys.currentSelectedGameObject.transform.IsChildOf(transform))
        {
            eventSys.SetSelectedGameObject(GameObject.Find("FirstSelected"));
        }
    }

    public void Open()
    {
        CloseOtherPanels();

        if (closed)
            StartCoroutine("SwitchState");
    }

    public void Close()
    {
        if(!closed)
            StartCoroutine("SwitchState");
    }


    private IEnumerator SwitchState()
    {
        float from = 500f;
        float to = openedPos;

        if (!closed)
        {
            from = openedPos;
            to = 500f;
        }

        rect.anchoredPosition = new Vector2(from, 0f);

        Vector2 speed = new Vector2(closed ? -transitionSpeed : transitionSpeed, 0f);

        bool wasInferior = rect.anchoredPosition.x < to;
        bool isInferior = rect.anchoredPosition.x < to;

        while (wasInferior == isInferior)
        {
            rect.anchoredPosition += speed;
            
            wasInferior = isInferior;
            isInferior = rect.anchoredPosition.x < to;
            
            yield return null;
        }

        closed = !closed;

        if(closed)
        {
            gameObject.SetActive(false);
        }
    }

    private void CloseOtherPanels()
    {
        UIPanel[] panels = transform.parent.GetComponentsInChildren<UIPanel>();

        foreach(UIPanel pan in panels)
        {
            if (pan != this)
                pan.Close();
        }
    }
}