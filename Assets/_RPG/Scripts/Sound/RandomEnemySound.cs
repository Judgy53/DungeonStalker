using UnityEngine;
using System.Collections;

public class RandomEnemySound : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private float normalVolume = 1.0f;

    [SerializeField]
    private float angryVolume = 1.0f;

    [SerializeField]
    private bool autoPlay = true;

    [SerializeField]
    private float minDelayBetweenSounds = 0f;

    [SerializeField]
    private float maxDelayBetweenSounds = 5f;

    private AudioClip LastPlayed = null;

    private bool gameStarted = false;

    private Sensor sensor;
    private bool hasTarget = false;

    private float cooldown = 0f;

    private void Start()
    {
        if (clips.Length == 0)
            return;

        sensor = GetComponentInChildren<Sensor>();

        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
    }

    private void Update()
    {
        UpdateTarget();

        if (!autoPlay || !gameStarted)
            return;

        cooldown = Mathf.Max(cooldown - Time.deltaTime, 0f);

        if(cooldown == 0f)
        {
            AudioSource source = PlayOneSound();

            cooldown = source.clip.length;
            if (!hasTarget)
                cooldown += Random.Range(minDelayBetweenSounds, maxDelayBetweenSounds);
        }
    }


    private void UpdateTarget()
    {
        if (sensor == null)
            return;

        if(sensor.GotVisual && !hasTarget)
            cooldown = 0f;

        hasTarget = sensor.GotVisual;
    }

    void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        GameManager.OnPlayerCreation -= GameManager_OnPlayerCreation; //safety

        gameStarted = true;

        cooldown = Random.Range(minDelayBetweenSounds, maxDelayBetweenSounds); // set a start delay to avoid all enemy's playing sound at the same time
    }

    public AudioSource PlayOneSound()
    {
        AudioClip clip = SelectNewClip();

        AudioSource source = AudioManager.PlaySfx(clip, transform, hasTarget ? angryVolume : normalVolume);

        return source;
    }

    private AudioClip SelectNewClip()
    {
        AudioClip newClip = null;

        do
        {
            int pos = Random.Range(0, clips.Length);
            newClip = clips[pos];
        } while (newClip == null || newClip == LastPlayed);

        LastPlayed = newClip;

        return newClip;
    }
}