using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TMButton : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int value;
    [Header("Button Animation Settings")]
    [SerializeField] private float bttnspeed = 0.1f;
    [SerializeField] private float moveDist = 0.0025f;
    [SerializeField] private float buttonPressedTime = 0.1f;
    //[Header("Component References")]
    //[SerializeField] private Keypad keypad;

    public Animator anim;
    public AudioClip doorCloseSound;
    public AudioClip doorOpenSound;
    public AudioClip tmSound;
    public ParticleSystem tmParticles;

    public int getValue()
    {
        return value;
    }

    public void PressButton()
    {
        if (!moving)
        {
            StartCoroutine(MoveSmooth());
            CloseTMDoor();
        }
    }
    private bool moving;

    private IEnumerator MoveSmooth()
    {

        moving = true;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = transform.localPosition + new Vector3(0, 0, moveDist);

        float elapsedTime = 0;
        while (elapsedTime < bttnspeed)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / bttnspeed);

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
        transform.localPosition = endPos;
        yield return new WaitForSeconds(buttonPressedTime);
        startPos = transform.localPosition;
        endPos = transform.localPosition - new Vector3(0, 0, moveDist);

        elapsedTime = 0;
        while (elapsedTime < bttnspeed)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / bttnspeed);

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
        transform.localPosition = endPos;

        moving = false;
    }

    public void CloseTMDoor()
    {
        tmParticles.Play();
        PlayDoorCloseSound();

        PlayTimeMachineSound();

        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        anim.SetTrigger("TM_Close");

        yield return new WaitForSeconds(3f);

        if (GameManager.Instance.GetHasDestroyedMurderWeapon())
        {
            GameManager.Instance.ReturnMurderWeapon();
        }

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
