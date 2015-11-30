using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInteractInfos : MonoBehaviour {

    private InteractManager interactManager = null;

    private Text textComponent = null;

    private void Start()
    {
        textComponent = GetComponent<Text>();

        GameManager.OnPlayerCreation += OnPlayerCreation;
    }

    private void OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        interactManager = e.player.GetComponent<InteractManager>();
    }

    private void Update()
    {
        string output = "";
        if (interactManager != null && interactManager.Target as Behaviour != null)
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
