using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonsSoundHandler : MonoBehaviour
{
    [SerializeField]
    private AudioClip rolloverClip;

    [SerializeField]
    private AudioClip clickClip;

    private GameObject lastSelected = null;

    // Update is called once per frame
    void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null 
            && lastSelected != null 
            && currentSelected != lastSelected 
            && currentSelected.transform.IsChildOf(transform))
        {
            Button currentBtn = currentSelected.GetComponent<Button>();
   
            if(currentBtn != null)
            {
                onBtnHover();
            }
        }

        lastSelected = currentSelected;
    }

    public void onBtnHover()
    {
        if(rolloverClip != null)
            AudioManager.PlaySfx(rolloverClip, Camera.main.transform);
    }

    public void onBtnClick()
    {
        if(clickClip != null)
            AudioManager.PlaySfx(clickClip, Camera.main.transform);
    }
}