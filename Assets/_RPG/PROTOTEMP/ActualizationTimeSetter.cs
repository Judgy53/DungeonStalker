using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActualizationTimeSetter : MonoBehaviour
{
    public Unit[] units = null;
    public InputField input = null;

    private void Start()
    {
        input.onValueChange.AddListener(OnValueChange);
    }

    private void OnValueChange(string value)
    {
        if (value.Length != 0)
            SetAllUnits(float.Parse(value));
        else
            SetAllUnits(0.3f);
    }

    private void SetAllUnits(float time)
    {
        foreach (var unit in units)
            unit.actualizationTime = time;
    }
}
