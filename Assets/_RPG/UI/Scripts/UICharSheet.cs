using UnityEngine;
using System;
using System.Collections;

public class UICharSheet : MonoBehaviour
{
    public event EventHandler<UIMenuStateChangeArgs> OnMenuStateChange;

    public GameObject content = null;

    [SerializeField]
    private GameObject popup;

    private UIMenuState state = UIMenuState.Hidden;
    public UIMenuState State { get { return state; }
        set
        {
            if (OnMenuStateChange != null)
                OnMenuStateChange(this, new UIMenuStateChangeArgs(value));

            state = value;
        }
    }

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

        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
        if (popup != null)
            popup.SetActive(false);
    }

    void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        StatsManager stats = e.player.GetComponent<StatsManager>();
        if(stats != null && popup != null)
        {
            stats.OnLevelUp += Player_OnLevelUp;
        }
    }

    void Player_OnLevelUp(object sender, EventArgs e)
    {
        if(!popup.activeInHierarchy)
            popup.SetActive(true);
    }

    //Must be called after all registrations are done (ScriptExecutionOrder)
    private void Start()
    {
        State = UIMenuState.Hidden;
        content.SetActive(false);
    }

    private void Update()
    {
        if (GameInput.GetButtonDown("ToggleCharSheet", true))
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

            if (popup != null && popup.activeInHierarchy)
                popup.SetActive(false);
        }
    }
}
