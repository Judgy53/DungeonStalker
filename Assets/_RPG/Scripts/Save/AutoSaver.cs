using UnityEngine;
using System.Collections.Generic;
using System;

public class AutoSaver : MonoBehaviour
{
    private static AutoSaver instance = null;

    public static event EventHandler OnAutoSave;
    
    [Tooltip("Seconds to wait between each auto save")]
    [SerializeField]
    private int saveEvery = 120;

    [Tooltip("In case the player is near enemies, report the save of that time (in seconds)")]
    [SerializeField]
    private int reportTime = 30;

    [Tooltip("Minimum Distance from enemies to be safe to save")]
    [SerializeField]
    private float safeDistance = 3f;

    private float timer = 0;

    private GameObject player = null;

    private bool registeredForPlayerCreation = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        GameManager.OnReloadStage += GameManager_OnReloadStage;
    }

    void Update()
    {
        if (player == null && !registeredForPlayerCreation) // if no player and not already waiting
        {
            GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
            registeredForPlayerCreation = true;
        }
        else if (player != null)
        {
            timer += Time.deltaTime;

            if (timer >= saveEvery)
                TrySave();
        }
    }

    private void TrySave()
    {
        List<GameObject> enemies = GameManager.MazeInstance.Enemies;

        bool safe = true;

        foreach(GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            if(Vector3.Distance(enemy.transform.position, player.transform.position) < safeDistance)
            {
                safe = false;
                break;
            }
        }

        if (safe)
        {
            Debug.Log("No enemies around, autosaving ...");

            if (OnAutoSave != null)
                OnAutoSave(this, new EventArgs());

            SaveManager.Instance.Save();
            timer = 0f;
        }
        else
        {
            Debug.Log("Not safe to save, reporting save.");
            timer = saveEvery - reportTime;
        }
    }

    void GameManager_OnReloadStage(object sender, System.EventArgs e)
    {
        player = null;

        registeredForPlayerCreation = false;
    }

    private void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        player = e.player;

        registeredForPlayerCreation = false;
    }

    public void Disable()
    {
        player = null;
        timer = 0f;
    }
}