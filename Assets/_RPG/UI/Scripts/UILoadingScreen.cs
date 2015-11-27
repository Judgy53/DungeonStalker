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
    
    private delegate void ActualizerBehavior();
    /// <summary>
    /// Coroutines can't do this job, needed a real thread.
    /// </summary>
    private System.Threading.Thread progressActualizerThread = null;
    private ActualizerBehavior actualizerBehavior = null;
    private bool quitThread = false;

    /// <summary>
    /// Unity has protections against multithreading, needed a middle variable.
    /// </summary>
    private string percentValue = "0";

    private void Start()
    {
        GameManager.OnMazeGenerationFinished += GameManager_OnMazeGenerationFinished;
        GameManager.OnMazePopulationFinished += GameManager_OnMazePopulationFinished;
        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;

        animator = GetComponent<Animator>();
        loaderActualizer = ActualizeLoadingSentence();
        textChanger = ChangeTextOnTime();
        progressActualizerThread = new System.Threading.Thread(ActualizeProgress);
        
        StartCoroutine(loaderActualizer);
        StartCoroutine(textChanger);
        progressActualizerThread.Start();

        UIStateManager.RegisterUI();
    }

    private void OnDestroy()
    {
        if (progressActualizerThread != null)
        {
            quitThread = true;
            progressActualizerThread.Join();
        }
    }

    private void Update()
    {
        lock (percentValue)
        {
            percent.text = percentValue;
        }
    }

    private IEnumerator ChangeTextOnTime()
    {
        while (true)
        {
            if (sentence != null)
                sentence.text = GetRandomSentence();
            yield return new WaitForSeconds(1.5f);
        }
    }

    private IEnumerator ActualizeLoadingSentence()
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

    private void ActualizeProgress()
    {
        actualizerBehavior = ActualizerBehaviorGeneration;

        while (!quitThread)
            actualizerBehavior();
    }

    private string GetRandomSentence()
    {
        return sentences[random.Next(sentences.Length)];
    }

    private void GameManager_OnMazeGenerationFinished(object sender, System.EventArgs e)
    {
        actualizerBehavior = ActualizerBehaviorPopulate;
    }

    private void GameManager_OnMazePopulationFinished(object sender, System.EventArgs e)
    {
        actualizerBehavior = ActualizerBehaviorPathfindingGeneration;
    }
    
    private void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        StopCoroutine(loaderActualizer);
        StopCoroutine(textChanger);
        quitThread = true;
        progressActualizerThread.Join();

        if (loadingSentence != null)
            loadingSentence.text = "Done !";

        if (animator != null)
            animator.SetTrigger("FadeOut");
    }

    private void ActualizerBehaviorGeneration()
    {
        float progress = 0.0f;
        int maxCells = GameManager.MazeInstance.Cells.Length;
        int initializedCells = 0;
        foreach (MazeCell cell in GameManager.MazeInstance.Cells)
        {
            if (cell != null && cell.IsFullyInitialized)
                initializedCells++;
        }

        progress = (float)initializedCells / (float)maxCells;
        progress *= 0.2f;

        ActualizeProgressText(progress);
    }

    private void ActualizerBehaviorPopulate()
    {
        float progress = (float)GameManager.MazeInstance.Enemies.Count / (float)GameManager.EnemiesNumber;
        progress *= 0.1f;
        progress += 0.2f;

        ActualizeProgressText(progress);
    }

    private void ActualizerBehaviorPathfindingGeneration()
    {
        float progress = 1.0f - ((float)Grid.WaitingRequestNumber / (float)Grid.HighestRequestNumber);
        progress *= 0.7f;
        progress += 0.3f;

        ActualizeProgressText(progress);
    }

    private void ActualizeProgressText(float progress)
    {
        percentValue = ((int)(progress * 100.0f)).ToString() + "%";
    }

    public void DisableCallback()
    {
        UIStateManager.UnregisterUI();
        gameObject.SetActive(false);
    }
}
