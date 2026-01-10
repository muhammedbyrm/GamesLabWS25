using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class TimeMachineButton : MonoBehaviour
{
    public Animator anim;
    public Collider buttonCollider;
    public AudioClip doorCloseSound;
    public AudioClip doorOpenSound;
    public AudioClip tmSound;
    public ParticleSystem tmParticles;

    public void CloseTMDoor()
    {
        buttonCollider.enabled = false;
        tmParticles.Play();
        PlayDoorCloseSound();

        PlayTimeMachineSound();

        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        anim.SetTrigger("TM_Close");

        yield return new WaitForSeconds(3f);

        buttonCollider.enabled = true;
        PlayDoorOpenSound();
    }

    public void PlayDoorCloseSound()
    {
        SoundManager.Instance.PlaySoundClip(doorCloseSound, transform, 1f);
    }

    public void PlayDoorOpenSound()
    {
        SoundManager.Instance.PlaySoundClip(doorOpenSound, transform, 1f);
    }

    public void PlayTimeMachineSound()
    {
        SoundManager.Instance.PlaySoundClipPitched(tmSound, transform, 1f, 3f);
    }
}
