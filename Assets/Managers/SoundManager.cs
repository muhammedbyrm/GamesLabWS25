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

        SetUpBGM();  
    }
    #endregion

    [SerializeField] private AudioSource sound;
    [SerializeField] private float music_volume;
    private AudioSource background_music_1;
    private AudioSource background_music_2;
    private AudioSource background_music_3;
    [SerializeField] private AudioClip present_background_music;
    [SerializeField] private AudioClip past_background_music;
    [SerializeField] private AudioClip finish_background_music;

    [SerializeField] private float sound_volume;


    private void PlayBGM(AudioClip bgm)
    {
        AudioSource audioSource = Instantiate(sound, Vector3.zero, Quaternion.identity);
        audioSource.clip = bgm;
        audioSource.volume = music_volume;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void SetUpBGM()
    {
        if (sound == null)
        {
            Debug.LogError("SoundManager: Audio Source did not found");
            return;
        }

        if (present_background_music == null || past_background_music == null || finish_background_music == null)
        {
            Debug.LogError("SoundManager: some files are missing");
            return;
        }

        background_music_1 = Instantiate(sound, Vector3.zero, Quaternion.identity);
        background_music_1.clip = present_background_music;
        background_music_1.volume = music_volume;
        background_music_1.loop = true;

        background_music_2 = Instantiate(sound, Vector3.zero, Quaternion.identity);
        background_music_2.clip = past_background_music;
        background_music_2.volume = music_volume;
        background_music_2.loop = true;

        background_music_3 = Instantiate(sound, Vector3.zero, Quaternion.identity);
        background_music_3.clip = finish_background_music;
        background_music_3.volume = music_volume;
        background_music_3.loop = true;
    }

    public void PlayBGMChoose(int bgm)
    {
        if (background_music_1 == null || background_music_2 == null || background_music_3 == null)
        {
            Debug.LogError("SoundManager: Background musics are not created yet!");
            return;
        }

        switch (bgm)
        {
            case 1:
                background_music_2.Stop();
                background_music_3.Stop();
                background_music_1.PlayDelayed(2f);
                break;
            case 2:
                background_music_1.Stop();
                background_music_3.Stop();
                background_music_2.PlayDelayed(5f);
                break;
            case 3:
                background_music_1.Stop();
                background_music_2.Stop();
                background_music_3.PlayDelayed(2f);
                break;
            case 4:
                background_music_1.Stop();
                background_music_2.Stop();
                background_music_3.Stop();
                break;
            default:
                Debug.Log("No such BGM");
                break;
        }
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