using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, ISavable
{
    private static GameManager instance = null;
    
    public Maze mazeInstance = null;
    public static Maze MazeInstance { get { return instance.mazeInstance; } }

    public GameObject playerPrefab = null;

    public static event EventHandler<EventPlayerCreationArgs> OnPlayerCreation;
    public static event EventHandler OnMazeGenerationFinished;
    public static event EventHandler OnMazePopulationFinished;

    /// <summary>
    /// Any scripts subscribing to this event is responsible for unsubscribing.
    /// (Example : You need to unsubscribe if your script is destroyed on a Application.LoadLevel()).
    /// </summary>
    public static event EventHandler OnReloadStage;

    [Range(0, 100)]
    public int minEnemies = 10;
    [Range(0, 100)]
    public int maxEnemies = 100;

    public int maxEnemiesPerRoom = 10;

    public bool debugSpawnBeforePathfinding = false;

    [SerializeField]
    private int seed = 42;
    public static int Seed { get { return instance.seed; } }

    [SerializeField]
    private uint stage = 1;
    public static uint Stage { get { return instance.stage; } set { instance.stage = value; } }

    private SaveData toLoad = null;

    private bool generateMaze = false;

    private DateTime startTime;
    public static DateTime StartTime { get { return instance.startTime; } set { instance.startTime = value; } }

    private long timePlayed;
    public static long TimePlayed
    {
        get
        {
            return instance.timePlayed += Convert.ToInt64((DateTime.Now - StartTime).TotalSeconds);
        }
        set
        {
            instance.timePlayed = value;
        }
    }

    private string playerName = "Player";
    public static string PlayerName { get { return instance.playerName; } set { instance.playerName = value; } }

    private int enemiesNumber = 0;
    public static int EnemiesNumber { get { return instance.enemiesNumber; } }

    /// <summary>
    /// Might move that to another Component ...
    /// </summary>
    private List<int> keys = new List<int>();
    public static List<int> Keys { get { return instance.keys; } }

    private Grid gridInstance = null;
    public static Grid GridInstance { get { return instance.gridInstance; } }

    private void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        instance = this;

        LoadStage(stage);

        ResetTime(0L);

        DontDestroyOnLoad(this.gameObject);
    }

    public static void ResetTime(long played)
    {
        StartTime = DateTime.Now;
        TimePlayed = played;
    }

    private void MazeGenerationFinished(bool manual)
    {
        Debug.Log("Finished generate !");

        gridInstance.RecomputeStaticObstacles(false);
        Task mazePopulation = new Task(mazeInstance.Populate(enemiesNumber, maxEnemiesPerRoom), false);
        mazePopulation.Finished += MazePopulationFinished;
        mazePopulation.Start();

        if (OnMazeGenerationFinished != null)
            OnMazeGenerationFinished(this, new EventArgs());
    }

    private void MazePopulationFinished(bool manual)
    {
        Debug.Log("Populate finished !");

        Grid.OnProcessingQueueEmpty += Grid_OnProcessingQueueEmpty;
        if (debugSpawnBeforePathfinding)
            Grid_OnProcessingQueueEmpty(null, new EventArgs());

        if (OnMazePopulationFinished != null)
            OnMazePopulationFinished(this, new EventArgs());
    }

    private void Grid_OnProcessingQueueEmpty(object sender, EventArgs args)
    {
        Debug.Log("Dynamic Obstacles pathfinding recalculation done !");

        Grid.OnProcessingQueueEmpty -= Grid_OnProcessingQueueEmpty;

        GameObject playerStart = GameObject.FindGameObjectWithTag("PlayerStart");
        if (playerStart == null)
        {
            Debug.LogError("No start created !");
            return;
        }

        if (playerPrefab != null)
        {
            Camera mc = Camera.main;
            if (mc == null)
                throw new InvalidOperationException("Fatal : No main camera found !");

            GameObject player = GameObject.Instantiate(playerPrefab, playerStart.transform.position, playerStart.transform.rotation) as GameObject;
            mc.transform.SetParent(player.transform, false);

            if (OnPlayerCreation != null)
                OnPlayerCreation(this, new EventPlayerCreationArgs(player));
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (generateMaze)
        {
            mazeInstance = gameObject.GetComponentWithTag<Maze>("Maze");
            gridInstance = mazeInstance.GetComponent<Grid>();

            if (mazeInstance != null)
            {
                mazeInstance.Seed = seed + (int)stage;
                Task mazeGeneration = new Task(mazeInstance.Generate(), false);
                mazeGeneration.Finished += MazeGenerationFinished;
                mazeGeneration.Start();
            }
            else
                throw new InvalidOperationException("Maze instance undefined.");

            generateMaze = false;
        }
    }

    public static void LoadStage(uint s)
    {
        if (OnReloadStage != null)
            OnReloadStage(instance, new EventArgs());

        OnPlayerCreation = null;
        OnMazeGenerationFinished = null;
        OnMazePopulationFinished = null;

        instance.stage = s;
        instance.generateMaze = true;

        instance.enemiesNumber = UnityEngine.Random.Range(instance.minEnemies, instance.maxEnemies);

        Application.LoadLevel(Application.loadedLevel);
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
