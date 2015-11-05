using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBar : MonoBehaviour
{
    private float currentValue = 0f;
    public float CurrentValue { get { return currentValue; } set { currentValue = value; } }

    private float maxValue = 1f;
    public float MaxValue { get { return maxValue; } set { maxValue = value; } }

    private Image bar = null;

    [SerializeField]
    private bool showValues = false;
    public bool ShowValues { get { return showValues; }
        set
        {
            if (value)
            {
                if (textgo != null)
                {
                    text = textgo.GetComponent<Text>();
                    text.text = (int)currentValue + "/" + (int)maxValue; ;
                }
                else
                {
                    textgo = new GameObject("Values", typeof(RectTransform), typeof(Text));
                    textgo.transform.SetParent(transform.parent, false);
                    textgo.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().sizeDelta.x / 10.0f, transform.parent.GetComponent<RectTransform>().sizeDelta.y);
                    text = textgo.GetComponent<Text>();
                    text.text = (int)currentValue + "/" + (int)maxValue;
                    text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    text.alignment = TextAnchor.MiddleCenter;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 10;
                    text.resizeTextMaxSize = int.MaxValue;
                    text.horizontalOverflow = HorizontalWrapMode.Overflow;
                }
            }
            else
            {
                if (textgo != null)
                    GameObject.Destroy(textgo);
            }
        }
    }

    public string prefix = "";
    public string suffix = "";

    private GameObject textgo = null;
    private Text text = null;

    private void Start()
    {
        bar = GetComponent<Image>();
        if (showValues)
            ShowValues = showValues;
    }

    private void Update()
    {
        Vector3 barScale = bar.transform.localScale;
        barScale.x = currentValue / maxValue;

        bar.transform.localScale = barScale;

        if (textgo != null && text != null && showValues)
            text.text = prefix + (int)currentValue + "/" + (int)maxValue + suffix;
    }
}
