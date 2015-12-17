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

    [SerializeField]
    private float musicFadingSpeed = 0.005f;

    private AudioSource currentMusic = null;
    private float currentMusicCurrentVolume = 0f;
    private float currentMusicDesiredVolume = 1f;

    private AudioSource fadingOutMusic = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    { 
        if(fadingOutMusic != null)
        {
            if (fadingOutMusic.volume > 0f)
                fadingOutMusic.volume -= musicFadingSpeed;
            else
            {
                Destroy(fadingOutMusic.gameObject);
                fadingOutMusic = null;
            }
        }

        if(currentMusic != null && currentMusicCurrentVolume < currentMusicDesiredVolume)
        {
            currentMusicCurrentVolume = Mathf.Min(currentMusicCurrentVolume + musicFadingSpeed, currentMusicDesiredVolume);
            currentMusic.volume = currentMusicCurrentVolume * MusicVolume;
        }

    }

    public static AudioSource PlaySfx(AudioClip clip, Transform emitter, float volume = 1.0f, bool loop = false)
    {
        GameObject gao = Instantiate<GameObject>(instance.audioSourcePrefab);
        gao.name = "SfxClip_" + clip.name;
        gao.transform.position = Vector3.zero;
        gao.transform.rotation = Quaternion.identity;
        gao.transform.SetParent(emitter, false);

        AudioSource source = gao.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * SfxVolume;
        source.loop = loop;

        source.Play();

        if(!loop)
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

    public static AudioSource PlayMusic(AudioClip clip, Transform emitter, float volume = 1.0f)
    {
        GameObject gao = Instantiate<GameObject>(instance.audioSourcePrefab);
        gao.name = "MusicClip_" + clip.name;
        gao.transform.position = Vector3.zero;
        gao.transform.rotation = Quaternion.identity;
        gao.transform.SetParent(emitter, false);

        AudioSource source = gao.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * MusicVolume;
        source.pitch = 1f;
        source.loop = true;

        //Setup music fading
        if(instance.currentMusic != null)
            instance.fadingOutMusic = instance.currentMusic;

        instance.currentMusic = source;
        instance.currentMusicCurrentVolume = -0.5f;
        instance.currentMusicDesiredVolume = volume;

        source.Play();

        DontDestroyOnLoad(gao);

        return source;
    }
}