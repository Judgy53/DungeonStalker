using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanel : MonoBehaviour
{
    [SerializeField]
    private Button linkedButton;

    private RectTransform rect;

    private bool closed = true;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        if(linkedButton != null)
        {
            linkedButton.onClick.AddListener(btnClick);
        }

        rect.anchoredPosition = new Vector2(500f, 0f);
    }

    private void btnClick()
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
        float to = 150f;

        if (!closed)
        {
            from = 150f;
            to = 500f;
        }

        rect.anchoredPosition = new Vector2(from, 0f);

        Vector2 speed = new Vector2(closed ? -25f : 25f, 0f);

        while (rect.anchoredPosition.x != to)
        {
            rect.anchoredPosition += speed;
            yield return null;
        }

        closed = !closed;
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