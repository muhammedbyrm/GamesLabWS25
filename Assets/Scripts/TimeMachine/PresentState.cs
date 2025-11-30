using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PresentState : State
{


    public override void Enter()
    {
        Debug.Log("Entered Present State");

        GameManager.Instance.pastStateTimerImage.gameObject.SetActive(false);
        GameManager.Instance.pastStateTimerImageBG.gameObject.SetActive(false);

        GameManager.Instance.sphere.SetActive(true);
        GameManager.Instance.cube.SetActive(false);
    }
    public override void Update()
    {
    }

    public override void Exit()
    {
        GameManager.Instance.TimeJump();
    }
}
