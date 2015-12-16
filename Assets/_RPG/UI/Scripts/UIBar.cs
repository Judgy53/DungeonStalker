using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UIBar : MonoBehaviour
{
    private float currentValue = 0f;
    public float CurrentValue { get { return currentValue; } set { currentValue = value; } }

    private float maxValue = 1f;
    public float MaxValue { get { return maxValue; } set { maxValue = value; } }

    private float prevValue = -1f;
    private float prevMaxValue = -1f;

    private Image bar = null;

    [SerializeField]
    private bool showValues = false;
    public bool ShowValues
    {
        get { return showValues; }
        set
        {
            if (value)
            {
                if (textgo != null)
                {
                    //text = textgo.GetComponent<Text>();
                    //text.text = (int)currentValue + "/" + (int)maxValue; ;
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

    [SerializeField]
    private string barType;

    private IQuantifiable target;

    private void Start()
    {
        bar = GetComponent<Image>();
        if (showValues)
            ShowValues = showValues;

        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
    }

    void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        if (barType == null || barType.Length == 0)
        {
            Debug.LogError(this.name + " has no type");
            return;
        }

        Type t = Type.GetType(barType); // get class type from string

        if (t == null) // type not found 
        {
            Debug.LogError(this.name + "'s type is invalid");
            return;
        }

        Component comp = e.player.GetComponent(t);

        if (comp == null) // component not found
        {
            Debug.LogError(this.name + "'s type does not exists on player");
            return;
        }

        if (!(comp is IQuantifiable)) // component not quantifiable
        {
            Debug.LogError(this.name + "'s type is not quantifiable");
            return;
        }

        target = comp as IQuantifiable;
    }

    private void Update()
    {
        if (target == null)
            return;

        currentValue = target.GetCurrentValue();
        maxValue = target.GetMaxValue();

        if(currentValue != prevValue || maxValue != prevMaxValue)
        {
            Vector3 barScale = bar.transform.localScale;
            barScale.x = Mathf.Max(currentValue / maxValue, 0f);

            bar.transform.localScale = barScale;

            if (textgo != null && text != null && showValues)
                text.text = prefix + (int)currentValue + "/" + (int)maxValue + suffix;
        }

        prevValue = currentValue;
        prevMaxValue = maxValue;
    }
}