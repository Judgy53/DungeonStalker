using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBar : MonoBehaviour
{
    private float currentValue = 1f;
    public float CurrentValue { get { return currentValue; } set { currentValue = value; } }

    private float maxValue = 1f;
    public float MaxValue { get { return maxValue; } set { maxValue = value; } }

    private Image bar = null;

    private void Start()
    {
        bar = GetComponent<Image>();
    }

    private void Update()
    {
        Vector3 barScale = bar.transform.localScale;
        barScale.x = currentValue / maxValue;

        bar.transform.localScale = barScale;
    }
}
