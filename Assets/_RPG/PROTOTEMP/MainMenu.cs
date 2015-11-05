using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

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
