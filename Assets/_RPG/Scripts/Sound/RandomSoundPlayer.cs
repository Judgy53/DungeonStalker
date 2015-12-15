using UnityEngine;
using System.Collections;

public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private float volume = 1.0f;

    [SerializeField]
    private bool autoPlay = true;

    [SerializeField]
    private float minDelayBetweenSounds = 0f;

    [SerializeField]
    private float maxDelayBetweenSounds = 5f;

    private AudioClip LastPlayed = null;

    private void Start()
    {
        if (clips.Length == 0)
            return;

        if (autoPlay)
            GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;
    }

    void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        GameManager.OnPlayerCreation -= GameManager_OnPlayerCreation;
        StartCoroutine("AutoPlay");
    }

    public AudioSource PlayOneSound()
    {
        AudioClip clip = SelectNewClip();

        AudioSource source = AudioManager.PlaySfx(clip, transform, volume);

        return source;
    }

    private IEnumerator AutoPlay()
    {
        yield return new WaitForSeconds(Random.Range(minDelayBetweenSounds, maxDelayBetweenSounds)); // don't play sound on start

        while (enabled)
        {
            AudioSource source = PlayOneSound();

            float length = source.clip.length;
            length += Random.Range(minDelayBetweenSounds, maxDelayBetweenSounds);

            yield return new WaitForSeconds(length);
        }
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