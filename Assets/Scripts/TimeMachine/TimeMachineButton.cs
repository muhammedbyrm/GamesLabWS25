using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections;

public class TimeMachineButton : MonoBehaviour
{
    public Animator anim;

    public void CloseTMDoor()
    {
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        anim.SetTrigger("TM_Close");

        yield return new WaitForSeconds(5f);
    }
}
