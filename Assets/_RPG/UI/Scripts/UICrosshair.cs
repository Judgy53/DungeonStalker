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

    private void OnStateChangeCallback(object sender, UIMenuStateChangeArgs args)
    {
        if (args.newState == UIMenuState.Hidden)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
