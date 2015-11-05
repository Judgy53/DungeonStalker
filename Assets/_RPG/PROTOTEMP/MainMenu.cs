using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public void LoadScene(int id)
    {
        Application.LoadLevel(id);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
