using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void LoadScene(int id)
    {
        Application.LoadLevel(id);
    }

    public void LoadScene(string name)
    {
        Application.LoadLevel(name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
