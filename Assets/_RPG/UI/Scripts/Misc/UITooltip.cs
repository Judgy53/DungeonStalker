using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(UIMouseEvents))]
public class UITooltip : MonoBehaviour
{
    public bool autoAddListeners = true;

    [SerializeField]
    private GameObject tooltipPrefab = null;

    private static Text text = null;
    public static string Text { get { return text.text; } set { text.text = value; } }

    private static GameObject tooltipInstance = null;
    private static RectTransform trn = null;

    private bool isOver = false;
    private static bool forceHide = false;

    public static void DisableForceHide()
    {
        forceHide = false;
    }

    public static void ForceHide()
    {
        forceHide = true;
        HideOnce();
    }

    public static void HideOnce()
    {
        if (tooltipInstance != null)
            tooltipInstance.SetActive(false);
    }

    private void Awake()
    {
        if (tooltipPrefab != null && tooltipInstance == null)
        {
            tooltipInstance = GameObject.Instantiate(tooltipPrefab) as GameObject;
            tooltipInstance.transform.SetParent(transform.root, false);
            tooltipInstance.transform.position = Vector3.zero;
            text = tooltipInstance.GetComponentInChildren<Text>();
            trn = tooltipInstance.GetComponent<RectTransform>();
            tooltipInstance.SetActive(false);
        }

        if (autoAddListeners)
        {
            UIMouseEvents me = GetComponent<UIMouseEvents>();
            me.onMouseEnter.AddListener(OnMouseEnter);
            me.onMouseExit.AddListener(OnMouseExit);
        }
    }

    private void Update()
    {
        if (forceHide)
            tooltipInstance.SetActive(false);

        if (isOver)
            trn.position = Input.mousePosition;
    }

    public void OnMouseEnter(UIMouseEvents sender)
    {
        if (!forceHide)
            tooltipInstance.SetActive(true);
        isOver = true;
    }

    public void OnMouseExit(UIMouseEvents sender)
    {
        tooltipInstance.SetActive(false);
        tooltipInstance.transform.position = Vector3.zero;
        isOver = false;
    }
}