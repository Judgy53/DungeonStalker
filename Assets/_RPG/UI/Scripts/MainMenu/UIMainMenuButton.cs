using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMainMenuButton : MonoBehaviour
{
    [SerializeField]
    private UIPanel linkedPanel;

    void Start()
    {
        Button btn = GetComponent<Button>();

        if (btn != null && linkedPanel != null)
        {
            btn.onClick.AddListener(OpenPanel);
        }
    }

    private void OpenPanel()
    {
        if(linkedPanel.gameObject.activeInHierarchy)
            return;

        linkedPanel.gameObject.SetActive(true);
        linkedPanel.Open();
    }
}