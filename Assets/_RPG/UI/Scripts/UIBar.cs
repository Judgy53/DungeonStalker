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
                    textgo.transform.SetParent(transform, false);
                    text = textgo.GetComponent<Text>();
                    text.text = (int)currentValue + "/" + (int)maxValue;
                }
            }
            else
            {
                if (textgo != null)
                    GameObject.Destroy(textgo);
            }
        }
    }

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
            text.text = (int)currentValue + "/" + (int)maxValue;
    }
}
