using UnityEngine;
using UnityEngine.UI;

public class PastState : State
{
    private float timeInState = 0f;
    private float loopTimer;
    private bool jumpSequenceStarted = false;

    public Image pastStateTimerImage;
    public Image pastStateTimerImageBG;
    private RectTransform pastStateTimerRect;
    private Color TMColor = new Color(8f / 255f, 218f / 255f, 238f / 255f);
    private float UIStartWidth;
    public override void Enter()
    {
        
        Debug.Log("Entered Past State");
        
        
        
        GameManager.Instance.laserPuzzle.ResetPuzzle();
        GameManager.Instance.ResetTerminalVisuals();
        loopTimer = GameManager.Instance.GetPastStateDuration();
        //GameManager.Instance.pastStateTimerImage.SetEnabled(true);
        pastStateTimerImage = GameManager.Instance.pastStateTimerImage;
        pastStateTimerImageBG = GameManager.Instance.pastStateTimerImageBG;
        pastStateTimerRect = pastStateTimerImage.GetComponent<RectTransform>();
        UIStartWidth = pastStateTimerRect.sizeDelta.x;

        GameManager.Instance.SetPastStateTimerVisible(true);
        GameManager.Instance.SetIsInThePast(true);

        //Preliminary setup for Past State
        GameManager.Instance.CloseBRDoor();
        GameManager.Instance.activatePastObjects();
        GameManager.Instance.ActivateKeypad();
        GameManager.Instance.DeactivateTMButton();

        SoundManager.Instance.PlayBGMChoose(2);

        //Add Sanity effect here later
        SanityManager.Instance.UpdateSanityEffects();
        //loop counter is in GameManager

        InteractionUnlocker.Instance.EnableInteractions();
        GameManager.Instance.SetPuzzleLights(true);


    }
    public override void Update()
    {

        //Debug.Log("Past State Timer: " + timeInState.ToString("F2") + " / " + loopTimer.ToString("F2"));

        //increment timer only when allowed
        if (GameManager.Instance.getIncrementPastTimer())
        {
            timeInState += Time.deltaTime;
        }
        float progress = Mathf.Clamp01(timeInState / loopTimer);
        float t = 1f - progress;

        if (GameManager.Instance.GetHasDestroyedMurderWeapon())
        {
            GameManager.Instance.SetPastStateTimerVisible(false);
        }
        else
        {
            pastStateTimerRect.sizeDelta = new Vector2(UIStartWidth * t, pastStateTimerRect.sizeDelta.y);
            //pastStateTimerImage.fillAmount = Mathf.Lerp(pastStateTimerImage.fillAmount, 1 - timeInState / loopTimer, Time.deltaTime * 10);
            pastStateTimerImage.color = Color.Lerp(TMColor, Color.red, timeInState / loopTimer);
        }

        if (timeInState >= loopTimer && !jumpSequenceStarted)
        {
            jumpSequenceStarted = true;
            GameManager.Instance.StartPastToPresentSequence();
            GameManager.Instance.SetPastStateTimerVisible(false);
        }
    }
    public override void Exit()
    {
        
        
        
        GameManager.Instance.laserPuzzle.ResetPuzzle();
        GameManager.Instance.ResetTerminalVisuals();
        GameManager.Instance.ReturnMurderWeapon();
        GameManager.Instance.deactivatePastObjects();
        GameManager.Instance.MovePlayer();
        if (GameManager.Instance.GetHasFlashlight())
        {
            GameManager.Instance.InteractFlashlight();
        }
        pastStateTimerRect.sizeDelta = new Vector2(UIStartWidth, pastStateTimerRect.sizeDelta.y);
        pastStateTimerImage.color = TMColor;
        //pastStateTimerImage.fillAmount = 1f;
    }
}
