using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField]
    private GameObject audioSourcePrefab;

    [SerializeField]
    private float sfxVolume = 1.0f;
    public static float SfxVolume { get { return instance.sfxVolume; } set { instance.sfxVolume = value; } }

    [SerializeField]
    private float musicVolume = 1.0f;
    public static float MusicVolume { get { return instance.musicVolume; } set { instance.musicVolume = value; } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public static AudioSource PlaySfx(AudioClip clip, Transform emitter, float volume = 1.0f)
    {
        GameObject gao = Instantiate<GameObject>(instance.audioSourcePrefab);
        gao.name = "SfxClip_" + clip.name;
        gao.transform.position = Vector3.zero;
        gao.transform.rotation = Quaternion.identity;
        gao.transform.SetParent(emitter, false);

        AudioSource source = gao.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * SfxVolume;

        source.Play();

        Destroy(gao, clip.length);

        return source;
    }

    public static AudioSource PlaySfx(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        GameObject gao = Instantiate<GameObject>(instance.audioSourcePrefab);
        gao.name = "SfxClip_" + clip.name;
        gao.transform.position = position;
        gao.transform.rotation = Quaternion.identity;

        AudioSource source = gao.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * SfxVolume;

        source.Play();

        Destroy(gao, clip.length);

        return source;
    }

    public static AudioSource PlayMusic(AudioClip clip, float volume = 1.0f)
    {
        GameObject gao = new GameObject("SfxClip_" + clip.name, typeof(AudioSource));
        gao.transform.position = Vector3.zero;
        gao.transform.rotation = Quaternion.identity;
        gao.transform.SetParent(Camera.main.transform, false);

        AudioSource source = gao.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * MusicVolume;
        source.pitch = 1f;

        source.Play();

        Destroy(gao, clip.length);

        return source;
    }
}