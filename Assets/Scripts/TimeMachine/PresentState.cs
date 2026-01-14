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

        if (GameManager.Instance.GetLoopCount() == 0)
        {
            //GameManager.Instance.CloseBRDoor();
            GameManager.Instance.deactivatePastObjects();
        }
        else
        {
            GameManager.Instance.OpenBRDoor(false);
            GameManager.Instance.LightOn();
            GameManager.Instance.StartFadeIn();
        }
        GameManager.Instance.activatePresentObjects();
        GameManager.Instance.DeactivateKeypad();
        GameManager.Instance.ActivateTMButton();

        SoundManager.Instance.PlayBGMChoose(1);

        InteractionUnlocker.Instance.DisableInteractions();
    }
    public override void Update()
    {
    }

    public override void Exit()
    {
        GameManager.Instance.deactivatePresentObjects();
        GameManager.Instance.TimeJump();
    }
}
