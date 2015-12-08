using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UIPauseMenu : MonoBehaviour
{
    public event EventHandler<UIMenuStateChangeArgs> OnMenuStateChange;

    private UIMenuState state = UIMenuState.Hidden;
    public UIMenuState State
    {
        get { return state; }
        set
        {
            if (OnMenuStateChange != null)
                OnMenuStateChange(this, new UIMenuStateChangeArgs(value));

            state = value;
        }
    }

    [SerializeField]
    private GameObject content;

    [SerializeField]
    private Button saveButton;

    [SerializeField]
    private Button quitButton;

    private void Awake()
    {
        //It must be fired first to enable all gameobjects before other events, hence registration in Awake().
        OnMenuStateChange += OnStateChangeCallback;

        if (content == null)
        {
            Debug.LogError("No content defined on " + this.name);
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        State = UIMenuState.Hidden;
        content.SetActive(false);

        saveButton.onClick.AddListener(SaveClick);
        quitButton.onClick.AddListener(QuitClick);
    }

    private void Update()
    {
        if (GameInput.GetButtonDown("Pause", true))
        {
            if (state == UIMenuState.Hidden && UIStateManager.State == UIState.Free)
                State = UIMenuState.Shown;
            else if (state == UIMenuState.Shown)
                State = UIMenuState.Hidden;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void OnStateChangeCallback(object sender, UIMenuStateChangeArgs args)
    {
        if (args.newState == UIMenuState.Hidden && State == UIMenuState.Shown)
        {
            UIStateManager.UnregisterUI();
            content.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else if (args.newState == UIMenuState.Shown && State == UIMenuState.Hidden)
        {
            UIStateManager.RegisterUI();
            content.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    private void SaveClick()
    {
        SaveManager.Instance.Save();
        State = UIMenuState.Hidden;
    }

    private void QuitClick()
    {
        SaveManager.Instance.Save();
        GameManager.GoToMainMenu();
    }
}
