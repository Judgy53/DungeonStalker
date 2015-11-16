using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UISaveList : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private UICurrentSave selectedSaveHandler;

    private void OnEnable()
    {
        ClearList();

        Dictionary<string, Save> saves = SaveManager.Instance.Saves;

        float height = Mathf.Min(saves.Count * 20f, 300f);

        RectTransform transform = GetComponent<RectTransform>();

        Vector2 size = transform.sizeDelta;
        size.y = height;
        transform.sizeDelta = size;

        int count = 0;

        foreach (KeyValuePair<string, Save> kvp in SaveManager.Instance.Saves)
        {
            GameObject newButton = Instantiate(buttonPrefab) as GameObject;
            UISaveButton button = newButton.GetComponent<UISaveButton>();

            if (kvp.Value.autoSave)
                button.SaveId.text = "Auto";
            else
                button.SaveId.text = kvp.Key.Replace("save", "");

            button.SaveName.text = "Stage " + kvp.Value.Stage;

            button.transform.SetParent(content);

            RectTransform rect = button.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0f, count++ * 20f);
        }
    }

    private void ClearList()
    {
        foreach (Transform tr in content)
            Destroy(tr.gameObject);
    }
}