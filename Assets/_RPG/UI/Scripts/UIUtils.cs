using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIUtils{

    public static void UpdateScroller(RectTransform content, RectTransform holder, RectTransform selected)
    {
        float scrolled = content.anchoredPosition.y;
        float holderHeight = holder.sizeDelta.y;

        float btnHeight = selected.sizeDelta.y;

        float pos = selected.GetSiblingIndex() * btnHeight;

        if (pos < scrolled + btnHeight) // if selected button overflow on top
        {
            content.anchoredPosition = new Vector2(0f, pos);
        }
        else if (pos + btnHeight > scrolled + holderHeight) // if selected button overflow on bottom
        {
            content.anchoredPosition = new Vector2(0f, pos - btnHeight * (holderHeight / btnHeight - 1));
        }
    }
}
