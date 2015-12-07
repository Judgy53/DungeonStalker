using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIItemPauseMenu : MonoBehaviour
{
    public event EventHandler<UIMenuStateChangeArgs> OnItemPauseMenuStateChange;

    public PlayerContainer target = null;

    public GameObject content = null;

    public UnityEngine.EventSystems.EventSystem eventSystem = null;

    private UIMenuState state = UIMenuState.Hidden;
    public UIMenuState State { get { return state; }
        set
        {
            if (OnItemPauseMenuStateChange != null)
                OnItemPauseMenuStateChange(this, new UIMenuStateChangeArgs(value));

            state = value;
        }
    }

    private void Awake()
    {
        //It must be fired first to enable all gameobjects before other events, hence registration in Awake().
        OnItemPauseMenuStateChange += OnStateChangeCallback;

        GameManager.OnPlayerCreation += OnPlayerCreation;

        if (content == null)
        {
            Debug.LogError("No content gameobject set on " + this.name);
            enabled = false;
        }
    }

    private void OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        target = e.player.GetComponentInChildren<PlayerContainer>();
    }

    //Must be called after all registrations are done (ScriptExecutionOrder)
    private void Start()
    {
        State = UIMenuState.Hidden;
        content.SetActive(false);
    }

    private void Update()
    {
        if (GameInput.GetButtonDown("ToggleInventory", true))
        {
            if (state == UIMenuState.Hidden && UIStateManager.State == UIState.Free)
                State = UIMenuState.Shown;
            else if (state == UIMenuState.Shown)
                State = UIMenuState.Hidden;
        }
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
}

public enum UIMenuState
{
    Hidden,
    Shown
}

public class UIMenuStateChangeArgs : EventArgs
{
    public UIMenuState newState;

    public UIMenuStateChangeArgs(UIMenuState newState)
    {
        this.newState = newState;
    }
}
