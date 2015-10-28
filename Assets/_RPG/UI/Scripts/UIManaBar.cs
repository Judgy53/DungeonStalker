using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManaBar : MonoBehaviour {

    [SerializeField]
    private ManaManager target = null;

    private Image manaBar = null;

    private void Start()
    {
        if (target == null)
            Debug.LogError(this.name + " is not set properly. Please bind the target.");

        manaBar = GetComponent<Image>();
    }

    private void Update()
    {
        float manaNormalized;

        if (target == null)
            manaNormalized = 0f;
        else
            manaNormalized = target.CurrentMana / target.MaxMana;

        Vector3 barScale = manaBar.transform.localScale;
        barScale.x = manaNormalized;

        manaBar.transform.localScale = barScale;
    }
}
