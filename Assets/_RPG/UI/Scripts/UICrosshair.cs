using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICrosshair : MonoBehaviour
{
    public UIItemPauseMenu pauseMenu = null;

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.OnItemPauseMenuStateChange += OnStateChangeCallback;
    }

    private void OnStateChangeCallback(object sender, ItemPauseMenuStateChangeArgs args)
    {
        if (args.newState == ItemPauseMenuState.Hidden)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
