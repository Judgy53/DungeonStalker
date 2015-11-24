using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelQuit : UIPanel
{
    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;

    void Start()
    {
        yesButton.onClick.AddListener(Quit);
        noButton.onClick.AddListener(Close);
    }

    private void Quit()
    {
        Application.Quit();
    }
}