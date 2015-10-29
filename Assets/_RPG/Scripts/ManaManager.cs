using UnityEngine;
using System.Collections;

public class ManaManager : MonoBehaviour
{

    [SerializeField]
    private float maxMana = 50.0f;
    public float MaxMana { get { return maxMana; } set { maxMana = value; } }

    private float currentMana = 50.0f;
    public float CurrentMana { get { return currentMana; } set { currentMana = value; } }

    [SerializeField]
    private float regenRate = 5.0f;
    public float RegenRate { get { return regenRate; } set { regenRate = value; } }

    private void Start()
    {
        currentMana = maxMana;
    }

    private void FixedUpdate()
    {
        currentMana += regenRate * Time.fixedDeltaTime;
        currentMana = Mathf.Min(currentMana, maxMana);
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }

    public void RemoveMana(float amount)
    {
        currentMana = Mathf.Min(currentMana - amount, maxMana);
    }
}
