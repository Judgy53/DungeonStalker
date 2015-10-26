using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIHealthBar : MonoBehaviour
{

    [SerializeField]
    private HealthManager target = null;

    private Image healthBar = null;

    private void Start()
    {
        if (target == null)
            Debug.LogError(this.name + " is not set properly. Please bind the target.");

        healthBar = GetComponent<Image>();
    }

    private void Update()
    {
        float healthNormalized;

        if (target == null)
            healthNormalized = 0f;
        else
            healthNormalized = target.CurrentHealth / target.MaxHealth;

        Vector3 barScale = healthBar.transform.localScale;
        barScale.x = healthNormalized;

        healthBar.transform.localScale = barScale;
    }
}
