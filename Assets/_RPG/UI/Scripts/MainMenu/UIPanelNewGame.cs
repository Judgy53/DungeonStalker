using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelNewGame : UIPanel
{
    [SerializeField]
    private Button createButton;

    [SerializeField]
    private Button cancelButton;

    void Start()
    {
        createButton.onClick.AddListener(Create);
        cancelButton.onClick.AddListener(Close);
    }

    private void Create()
    {
        //Create Game with Seed and player Name
        Application.LoadLevel("GameScene");
    }
}