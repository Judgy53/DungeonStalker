using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GenerateButton : MonoBehaviour
{
    public Maze maze = null;
    public InputField seed = null;
    public InputField sizex = null;
    public InputField sizey = null;

    public Button random;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        sizex.onValueChange.AddListener(OnSizeXValueChange);
        sizey.onValueChange.AddListener(OnSizeYValueChange);

        random.onClick.AddListener(RandomSeed);
    }

    void OnSizeXValueChange(string value)
    {
        if (int.Parse(sizex.text) > 50)
            sizex.text = "50";
    }

    void OnSizeYValueChange(string value)
    {
        if (int.Parse(sizey.text) > 50)
            sizey.text = "50";
    }

    void OnClick()
    {
        if (seed.text.Length == 0)
            RandomSeed();

        maze.seed = int.Parse(seed.text);

        if (sizex.text.Length != 0 && int.Parse(sizex.text) != 0)
            maze.size.x = int.Parse(sizex.text);
        else
            maze.size.x = 1;

        if (sizey.text.Length != 0 && int.Parse(sizey.text) != 0)
            maze.size.z = int.Parse(sizey.text);
        else
            maze.size.z = 1;

        maze.Regenerate();
    }

    void RandomSeed()
    {
        seed.text = Random.Range(0, 100000).ToString();
    }
}
