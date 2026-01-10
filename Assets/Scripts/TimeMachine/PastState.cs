using UnityEngine;
using UnityEngine.UI;

public class PastState : State
{
    private float timeInState = 0f;
    private float loopTimer;

    public Image pastStateTimerImage;
    public Image pastStateTimerImageBG;


    public override void Enter()
    {
        Debug.Log("Entered Past State");
        loopTimer = GameManager.Instance.GetPastStateDuration();
        //GameManager.Instance.pastStateTimerImage.SetEnabled(true);
        pastStateTimerImage = GameManager.Instance.pastStateTimerImage;
        pastStateTimerImageBG = GameManager.Instance.pastStateTimerImageBG;
        GameManager.Instance.pastStateTimerImage.gameObject.SetActive(true);
        GameManager.Instance.pastStateTimerImageBG.gameObject.SetActive(true);

        //Preliminary setup for Past State
        GameManager.Instance.CloseBRDoor();
        GameManager.Instance.activatePastObjects();
        GameManager.Instance.ActivateKeypad();
        GameManager.Instance.DeactivateTMButton();

        //Add Sanity effect here later
        SanityManager.Instance.UpdateSanityEffects();
        //loop counter is in GameManager
    }
    public override void Update()
    {
        //pastStateTimerImage.fillAmount = 1 - timeInState / loopTimer;
        pastStateTimerImage.fillAmount = Mathf.Lerp(pastStateTimerImage.fillAmount, 1 - timeInState / loopTimer, Time.deltaTime * 10);
        pastStateTimerImage.color = Color.Lerp(Color.red, Color.green, 1 - timeInState / loopTimer);






        timeInState += Time.deltaTime;
        if (timeInState >= loopTimer)
        {
            GameManager.Instance.stateMachine.ChangeState(new PresentState());
        }
    }

    public override void Exit()
    {
        GameManager.Instance.ReturnMurderWeapon();
        GameManager.Instance.deactivatePastObjects();
        pastStateTimerImage.fillAmount = 1f;
    }
}
