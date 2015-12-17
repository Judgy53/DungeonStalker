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
    public static event EventHandler OnMazeChestGenerationFinished;

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

    [Range(1, 10)]
    public int minChestNumber = 1;
    [Range(1, 10)]
    public int maxChestNumber = 10;
    public int maxChestNumberPerRoom = 2;

    public bool debugSpawnBeforePathfinding = false;

    [SerializeField]
    private int seed = 42;
    public static int Seed { get { return instance.seed; } set { instance.seed = value; }}

    [SerializeField]
    private uint stage = 1;
    public static uint Stage { get { return instance.stage; } set { instance.stage = value; } }

    [SerializeField]
    private AutoSaver autoSaver = null;

    private SaveData toLoad = null;

    private DateTime startTime;
    public static DateTime StartTime { get { return instance.startTime; } set { instance.startTime = value; } }

    private int timePlayed;
    public static int TimePlayed { get { return instance.timePlayed; } set { instance.timePlayed = value; } }

    private string playerName = "Player";
    public static string PlayerName { get { return instance.playerName; } set { instance.playerName = value; } }

    private string gameId = null;
    public static string GameId 
    {
        get 
        {
            if (instance.gameId == null)
                instance.gameId = Guid.NewGuid().ToString();

            return instance.gameId;
        }

        set { instance.gameId = value; }
    }

    private int enemiesNumber = 0;
    public static int EnemiesNumber { get { return instance.enemiesNumber; } }
    private int chestNumber = 0;

    private bool generateMaze = false;

    private Grid gridInstance = null;
    public static Grid GridInstance { get { return instance.gridInstance; } }

    private GameObject player = null;

    [SerializeField]
    private AudioClip menuClip;

    [SerializeField]
    private AudioClip inGameClip;

    private void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        instance = this;

        //LoadStage(stage);
        GoToMainMenu();

        DontDestroyOnLoad(this.gameObject);
    }

    public static void ResetTime(int played)
    {
        StartTime = DateTime.Now;
        TimePlayed = played;
    }

    private IEnumerator UpdateTimePlayed()
    {
        while (true) // must use StopCoroutine to stop the loop
        {
            timePlayed += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    private void MazeGenerationFinished(bool manual)
    {
        Debug.Log("Finished generate !");

        gridInstance.RecomputeStaticObstacles(false);

        if(toLoad == null)
        {
            Task mazePopulation = new Task(mazeInstance.Populate(enemiesNumber, maxEnemiesPerRoom), false);
            mazePopulation.Finished += MazePopulationFinished;
            mazePopulation.Start();
        }

        if (OnMazeGenerationFinished != null)
            OnMazeGenerationFinished(this, new EventArgs());
    }

    private void MazePopulationFinished(bool manual)
    {
        Debug.Log("Populate finished !");
        
        if (OnMazePopulationFinished != null)
            OnMazePopulationFinished(this, new EventArgs());

        if (toLoad == null)
        {
            Task mazeChestGeneration = new Task(mazeInstance.GenerateChests(chestNumber, maxChestNumberPerRoom), false);
            mazeChestGeneration.Finished += MazeChestGenerationFinished;
            mazeChestGeneration.Start();
        }
        else
        {
            Grid.OnProcessingQueueEmpty += Grid_OnProcessingQueueEmpty;

            if (debugSpawnBeforePathfinding)
                Grid_OnProcessingQueueEmpty(null, new EventArgs());
            if (OnMazeChestGenerationFinished != null)
                OnMazeChestGenerationFinished(this, new EventArgs());
            Debug.Log("Chests generation finished !");
        }

        toLoad = null; // reset this to ensure it won't be loaded at next load/level
    }

    private void MazeChestGenerationFinished(bool manual)
    {
        Debug.Log("Chests generation finished !");

        Grid.OnProcessingQueueEmpty += Grid_OnProcessingQueueEmpty;
        
        List<Container> chests = mazeInstance.Chests;
        //Generate loot.

        if (debugSpawnBeforePathfinding)
            Grid_OnProcessingQueueEmpty(null, new EventArgs());

        if (OnMazeChestGenerationFinished != null)
            OnMazeChestGenerationFinished(this, new EventArgs());
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

            if (player == null)
                player = GameObject.Instantiate(playerPrefab, playerStart.transform.position, playerStart.transform.rotation) as GameObject;
            else
            {
                player.transform.position = playerStart.transform.position;
                player.transform.rotation = playerStart.transform.rotation;
            }

            mc.transform.SetParent(player.transform.Find("CameraPoint"), false);

            ResetTime(0);

            if (OnPlayerCreation != null)
                OnPlayerCreation(this, new EventPlayerCreationArgs(player));

            StartCoroutine("UpdateTimePlayed");

            AudioManager.PlayMusic(instance.inGameClip, Camera.main.transform);
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

        UIStateManager.ClearState();
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

        instance.enemiesNumber = UnityEngine.Random.Range(instance.minEnemies, instance.maxEnemies + 1);
        instance.chestNumber = UnityEngine.Random.Range(instance.minChestNumber, instance.maxChestNumber + 1);

        if (instance.player != null)
        {
            DontDestroyOnLoad(instance.player);

            Camera.main.transform.SetParent(null, false);
        }

        Application.LoadLevel("GameScene");
    }

    public static void GoToMainMenu()
    {
        Camera.main.transform.SetParent(null ,false);

        if(instance != null) 
        {
            if(instance.autoSaver != null)
                instance.autoSaver.Disable();

            if(instance.player != null)
            {
                Destroy(instance.player);
                instance.player = null;
            }

            instance.StopCoroutine("UpdateTimePlayed");

            AudioManager.PlayMusic(instance.menuClip, Camera.main.transform);
       }

       Application.LoadLevel("MainMenu");
    }

    public void Save(SaveData data)
    {
        data.Add("seed", seed);
        data.Add("stage", stage);
        mazeInstance.Save(data);
    }

    public void Load(SaveData data)
    {
        Debug.Log("Loading GameManager");

        int loadedSeed = int.Parse(data.Get("seed"));
        uint stage = uint.Parse(data.Get("stage"));

        enemiesNumber = int.Parse(data.Get("enemyCount")); //for loading counter

        toLoad = data;

        seed = loadedSeed;
        LoadStage(stage);

        OnMazeGenerationFinished += MazeLoadFinished;
    }

    private void MazeLoadFinished(object sender, EventArgs e)
    {
        mazeInstance.GetComponent<Grid>().RecomputeStaticObstacles(false);

        //mazeInstance.GenerateEnemiesFromSave(toLoad);

        Task mazePopulation = new Task(mazeInstance.Load(toLoad), false);
        mazePopulation.Finished += MazePopulationFinished;
        mazePopulation.Start();
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
