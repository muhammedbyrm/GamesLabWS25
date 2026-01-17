using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PresentState : State
{

    public override void Enter()
    {
        Debug.Log("Entered Present State");


        GameManager.Instance.SetPastStateTimerVisible(false);
        GameManager.Instance.SetIsInThePast(false);

        if (GameManager.Instance.GetLoopCount() == 0)
        {
            //GameManager.Instance.CloseBRDoor();
            GameManager.Instance.deactivatePastObjects();
            GameManager.Instance.UIstartGame();
        }
        else
        {
            GameManager.Instance.OpenBRDoor(false);
            GameManager.Instance.LightOn();
            GameManager.Instance.StartFadeIn();
        }

        if(GameManager.Instance.GetLoopCount() == 1)
        {
            GameManager.Instance.UIAfterFirstTimeJump();
        }
        else if(GameManager.Instance.GetLoopCount() == 2)
        {
            GameManager.Instance.UIAfterSecondTimeJump();
        }
        else if(GameManager.Instance.GetLoopCount() == 3)
        {
            GameManager.Instance.UIAfterThirdTimeJump();
        }

        GameManager.Instance.activatePresentObjects();
        GameManager.Instance.DeactivateKeypad();
        GameManager.Instance.ActivateTMButton();

        if (GameManager.Instance.GetHasDestroyedMurderWeapon())
        {
            SoundManager.Instance.PlayBGMChoose(3);
        }
        else
        {
            SoundManager.Instance.PlayBGMChoose(1);
        }

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
