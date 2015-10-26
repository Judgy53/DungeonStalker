using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInteractInfos : MonoBehaviour {

    [SerializeField]
    private InteractManager interactManager = null;

    private Text textComponent = null;

    private void Start()
    {
        if(interactManager == null)
        {
            Debug.LogWarning(this.name + " is not set properly. Please bind the players's InteractManager");
        }

        textComponent = GetComponent<Text>();
    }

    private void Update()
    {
        string output = "";
        if (interactManager != null && interactManager.Target != null)
        {
            output = "E - ";

            output += interactManager.Target.GetActionName();
            output += "\n";
            output += interactManager.Target.GetDescription();
        }
        else
            output = "";

        textComponent.text = output;
    }
}
