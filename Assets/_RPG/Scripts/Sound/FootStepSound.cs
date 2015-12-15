using UnityEngine;
using System.Collections;

public class FootStepSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private bool playAuto = true;

    [SerializeField]
    private float delay = 0.5f;

    [SerializeField]
    private float volume = 1.0f;

    private Vector3 prevPos = Vector3.zero;

    private float height = 0f;

    private float cooldown = 0f;

    private bool isSprinting = false;
    public bool IsSprinting { get { return isSprinting; } set { isSprinting = value; } }

	void Start () {
        prevPos = transform.position;

        height = GetComponent<Collider>().bounds.extents.y;
	}
	
	void Update () {
        if (!playAuto)
            return;

        cooldown = Mathf.Max(cooldown - Time.deltaTime, 0f);

        if(cooldown == 0f && Vector3.Distance(prevPos, transform.position) > 0.1f) // if has moved
        {
            if(Physics.Raycast(transform.position, Vector3.down, height + 0.1f)) // if is on ground
            {
                PlayOneStep();

                cooldown = delay;

                if (isSprinting)
                    cooldown /= 2f;

                prevPos = transform.position;
            }
        }
	}

    public void PlayOneStep()
    {
        if (clips.Length == 0 || !enabled)
            return;

        AudioClip clip = SelectNewClip();

        AudioSource source = AudioManager.PlaySfx(clip, transform, volume);
    }

    private AudioClip SelectNewClip()
    {
        int pos = Random.Range(0, clips.Length);

        AudioClip newClip = clips[pos];

        return newClip;
    }
}
