using UnityEngine;
using System.Collections;

public class OnHitSound : MonoBehaviour
{
    private float cooldown = 0f;

    [SerializeField]
    private AudioClip[] hitSounds;

    void Start()
    {
        HealthManager health = GetComponent<HealthManager>();

        if(health != null)
            health.OnHit += health_OnHit;
    }

    void health_OnHit(object sender, System.EventArgs e)
    {
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0)
            return;

        int tabPos = Random.Range(0, hitSounds.Length);
        AudioClip clip = hitSounds[tabPos];

        AudioManager.PlaySfx(clip, transform);

        cooldown = clip.length + 0.5f;
    }

    void Update()
    {
        cooldown = Mathf.Max(cooldown - Time.deltaTime, 0f);
    }
}