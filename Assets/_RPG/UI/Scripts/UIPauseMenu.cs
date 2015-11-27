using UnityEngine;
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
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
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

    public void MainMenu()
    {
        SaveManager.Instance.Save();
        Application.LoadLevel("MainMenu");
    }
}
