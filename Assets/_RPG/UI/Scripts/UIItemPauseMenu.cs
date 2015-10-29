using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIItemPauseMenu : MonoBehaviour
{
    public event EventHandler<ItemPauseMenuStateChangeArgs> OnItemPauseMenuStateChange;

    public PlayerContainer target = null;

    public GameObject content = null;

    public UnityEngine.EventSystems.EventSystem eventSystem = null;
    public GUIElement firstFocus = null;

    private ItemPauseMenuState state = ItemPauseMenuState.Hidden;
    public ItemPauseMenuState State { get { return state; }
        set
        {
            if (OnItemPauseMenuStateChange != null)
                OnItemPauseMenuStateChange(this, new ItemPauseMenuStateChangeArgs(value));

            state = value;
        }
    }

    private void Awake()
    {
        //It must be fired first to enable all gameobjects before other events, hence registration in Awake().
        OnItemPauseMenuStateChange += OnStateChangeCallback;

        if (target == null)
        {
            Debug.LogError("No target set on " + this.name);
            enabled = false;
        }

        if (content == null)
        {
            Debug.LogError("No content gameobject set on " + this.name);
            enabled = false;
        }
    }

    //Must be called after all registrations are done (ScriptExecutionOrder)
    private void Start()
    {
        State = ItemPauseMenuState.Hidden;
    }

    private void Update()
    {
        if (Input.GetButtonDown("ToggleInventory"))
        {
            if (state == ItemPauseMenuState.Hidden)
                State = ItemPauseMenuState.Shown;
            else
                State = ItemPauseMenuState.Hidden;
        }
    }

    private void OnStateChangeCallback(object sender, ItemPauseMenuStateChangeArgs args)
    {
        if (args.newState == ItemPauseMenuState.Hidden)
        {
            content.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            /*if (eventSystem != null && firstFocus != null)
                eventSystem.SetSelectedGameObject(firstFocus.gameObject);*/
            content.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }
}

public enum ItemPauseMenuState
{
    Hidden,
    Shown
}

public class ItemPauseMenuStateChangeArgs : EventArgs
{
    public ItemPauseMenuState newState;

    public ItemPauseMenuStateChangeArgs(ItemPauseMenuState newState)
    {
        this.newState = newState;
    }
}
