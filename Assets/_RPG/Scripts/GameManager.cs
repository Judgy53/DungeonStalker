using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour, ISavable
{
    public Maze mazeInstance = null;

    public GameObject playerPrefab = null;

    public static event EventHandler<EventPlayerCreationArgs> OnPlayerCreation;

    [Range(0, 100)]
    public int minEnemies = 10;
    [Range(0, 100)]
    public int maxEnemies = 100;

    public int maxEnemiesPerRoom = 10;

    [SerializeField]
    private uint stage = 1;
    public static uint Stage { get { return instance.stage; } }

    private static GameManager instance = null;

    private SaveData toLoad = null;

    private void Start()
    {
        instance = this;

        if (mazeInstance != null)
        {
            Task mazeGeneration = new Task(mazeInstance.Generate(), false);
            mazeGeneration.Finished += MazeGenerationFinished;
            mazeGeneration.Start();
        }
    }

    private void MazeGenerationFinished(bool manual)
    {
        Debug.Log("Finished generate !");

        mazeInstance.GetComponent<Grid>().RecomputeStaticObstacles(false);
        Task mazePopulation = new Task(mazeInstance.Populate(UnityEngine.Random.Range(minEnemies, maxEnemies), maxEnemiesPerRoom), false);
        mazePopulation.Finished += MazePopulationFinished;
        mazePopulation.Start();
    }

    private void MazePopulationFinished(bool manual)
    {
        Debug.Log("Populate finished !");

        GameObject playerStart = GameObject.FindGameObjectWithTag("PlayerStart");
        if (playerStart == null)
        {
            Debug.LogError("No start created !");
            return;
        }

        if (playerPrefab != null)
        {
            GameObject player = GameObject.Instantiate(playerPrefab, playerStart.transform.position, playerStart.transform.rotation) as GameObject;

            if (OnPlayerCreation != null)
                OnPlayerCreation(this, new EventPlayerCreationArgs(player));
        }
    }

    private void Update()
    {

    }

    public void Save(SaveData data)
    {
        data.Add("stage", stage);
        mazeInstance.Save(data);
    }

    public void Load(SaveData data)
    {
        int seed = int.Parse(data.Get("seed"));

        toLoad = data;

        if(mazeInstance.Seed != seed)
        {
            mazeInstance.Seed = seed;

            mazeInstance.Clear();

            Task mazeGeneration = new Task(mazeInstance.Generate(), false);
            mazeGeneration.Finished += MazeLoadFinished;
            mazeGeneration.Start();
        }
        else
            MazeLoadFinished(true);
    }

    private void MazeLoadFinished(bool manual)
    {
        mazeInstance.GetComponent<Grid>().RecomputeStaticObstacles(false);

        mazeInstance.GenerateEnemiesFromSave(toLoad);

        toLoad = null;
    }
}

public class EventPlayerCreationArgs : EventArgs
{
    public GameObject player;

    public EventPlayerCreationArgs(GameObject gao)
    {
        player = gao;
    }
}
