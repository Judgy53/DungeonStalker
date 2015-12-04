using UnityEngine;
using System.Collections;

public class DebugSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs = new GameObject[0];

    private GameObject player = null;

    private void Start()
    {
        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
    }

    private void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        player = e.player;
    }

    public void SpawnCallback(int value)
    {
        if (player != null)
            GameObject.Instantiate(itemPrefabs[value], player.transform.position + player.transform.forward * 0.2f, Quaternion.identity);
    }
}
