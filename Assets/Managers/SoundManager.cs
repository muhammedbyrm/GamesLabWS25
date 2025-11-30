using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    #region SoundManager

    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("SoundManager is NUll");
            }
            return _instance;
        }
    }


    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        Debug.Log("Hello I am your personal SoundManager!");
    }
    #endregion

    [SerializeField] private AudioSource sound;
    [SerializeField] private float music_volume;
    [SerializeField] private AudioClip background_music;

    [SerializeField] private float sound_volume;


    private void Start()
    {
        //PlayBGM(background_music);
    }
    private void PlayBGM(AudioClip bgm)
    {
        AudioSource audioSource = Instantiate(sound, Vector3.zero, Quaternion.identity);
        audioSource.clip = bgm;
        audioSource.volume = music_volume;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlaySoundClip(AudioClip audioClip, Transform position, float volume)
    {
        AudioSource audioSource = Instantiate(sound, position.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float length = audioSource.clip.length;

        Destroy(audioSource.gameObject, length);
    }

    public void PlaySoundClipPitched(AudioClip audioClip, Transform position, float volume, float maximumLength)
    {
        AudioSource audioSource = Instantiate(sound, position.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        float pitch = audioSource.clip.length / maximumLength;

        audioSource.pitch = pitch;

        audioSource.Play();

        Destroy(audioSource.gameObject, maximumLength);
    }
}

