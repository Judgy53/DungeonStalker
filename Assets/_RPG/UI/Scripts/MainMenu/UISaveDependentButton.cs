using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISaveDependentButton : MonoBehaviour
{
    void Start()
    {
        if (SaveManager.Instance.Saves.Count == 0)
        {
            GetComponent<Button>().interactable = false;

            GetComponentInChildren<Text>().color = Color.grey;
        }
    }
}