using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelContinue : UIPanel
{
    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;

    void Start()
    {
        yesButton.onClick.AddListener(Continue);
        noButton.onClick.AddListener(Close);
    }
    private void Continue()
    {
        SaveManager.Instance.LoadLast();
    }
}