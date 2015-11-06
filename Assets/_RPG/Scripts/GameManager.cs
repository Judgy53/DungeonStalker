using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Maze mazeInstance = null;

    public GameObject playerPrefab = null;

    [Range(0, 100)]
    public int minEnemies = 10;
    [Range(0, 100)]
    public int maxEnemies = 100;

    public int maxEnemiesPerRoom = 10;

    private void Start()
    {
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

        mazeInstance.GetComponent<Grid>().RecomputeStaticObstacles();
        Task mazePopulation = new Task(mazeInstance.Populate(Random.Range(minEnemies, maxEnemies), maxEnemiesPerRoom), false);
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
            GameObject.Instantiate(playerPrefab, playerStart.transform.position, playerStart.transform.rotation);
    }

    private void Update()
    {

    }
}
