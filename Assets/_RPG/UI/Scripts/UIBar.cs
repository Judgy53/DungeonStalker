using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBar : MonoBehaviour
{
    private float valueNormalized = 0f;
    public float Value { get { return valueNormalized; } set { valueNormalized = value; } }

    private Image bar = null;

    private void Start()
    {
        bar = GetComponent<Image>();
    }

    private void Update()
    {
        Vector3 barScale = bar.transform.localScale;
        barScale.x = valueNormalized;

        bar.transform.localScale = barScale;
    }
}
