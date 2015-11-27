using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelLoad : UIPanel
{
    private string saveId = null;
    public string SaveId { get { return saveId; } set { saveId = value; } }

    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private Button cancelButton;

    private void Start()
    {
        loadButton.onClick.AddListener(Load);
        cancelButton.onClick.AddListener(Close);
    }

    private void Load()
    {
        if (saveId != null)
            SaveManager.Instance.Load(saveId);
    }
}