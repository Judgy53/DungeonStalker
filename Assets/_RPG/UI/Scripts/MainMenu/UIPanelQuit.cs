using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelQuit : MonoBehaviour
{
    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;

    void Start()
    {
        yesButton.onClick.AddListener(Application.Quit);
        noButton.onClick.AddListener(GetComponent<UIPanel>().Close);
    }
}