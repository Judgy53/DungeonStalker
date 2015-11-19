using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILoadingScreen : MonoBehaviour
{
    public Text loadingSentence = null;
    public Text sentence = null;
    public Text percent = null;

    public string[] sentences = { "Meh." };

    private System.Random random = new System.Random();

    private Animator animator = null;

    private int points = 0;
    public int Points { get { return points; }
        set
        {
            if (points + value > 5)
                points = 0;
            else
                points = value;
        }
    }

    private IEnumerator loaderActualizer = null;
    private IEnumerator textChanger = null;

    private void Start()
    {
        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
        animator = GetComponent<Animator>();
        loaderActualizer = ActualizeLoadingSentence();
        textChanger = ChangeTextOnTime();
        StartCoroutine(loaderActualizer);
        StartCoroutine(textChanger);
        UIStateManager.RegisterUI();
    }

    IEnumerator ChangeTextOnTime()
    {
        while (true)
        {
            if (sentence != null)
                sentence.text = GetRandomSentence();
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator ActualizeLoadingSentence()
    {
        while (true)
        {
            if (loadingSentence != null)
            {
                loadingSentence.text = "Loading stage " + GameManager.Stage + " ";
                for (int i = 0; i < points; i++)
                    loadingSentence.text += ".";
            }
            Points++;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private string GetRandomSentence()
    {
        return sentences[random.Next(sentences.Length)];
    }

    void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        StopCoroutine(loaderActualizer);
        StopCoroutine(textChanger);

        if (loadingSentence != null)
            loadingSentence.text = "Done !";

        if (animator != null)
            animator.SetTrigger("FadeOut");
        UIStateManager.UnregisterUI();
    }

    public void DisableCallback()
    {
        gameObject.SetActive(false);
    }
}
