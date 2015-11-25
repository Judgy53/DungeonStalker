using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelNewGame : UIPanel
{
    [SerializeField]
    private Button createButton;

    [SerializeField]
    private Button cancelButton;

    [SerializeField]
    private InputField nameField;

    [SerializeField]
    private InputField seedField;

    void Start()
    {
        createButton.onClick.AddListener(Create);
        cancelButton.onClick.AddListener(Close);
    }

    private void Create()
    {
        //Create Game with Seed and player Name
        string playerName = nameField.text;

        if (playerName.Length == 0)
            playerName = "Player";

        int seed = 0;

        if (!int.TryParse(seedField.text, out seed))
            seed = Random.Range(0, 1000000000);

        GameManager.PlayerName = playerName;
        GameManager.Seed = seed;

        GameManager.LoadStage(1); //stage must be at least 1
    }
}