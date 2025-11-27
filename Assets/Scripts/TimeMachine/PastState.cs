using UnityEngine;

public class PastState : State
{
    public PastState(Player player) : base(player) { }

    private float timeInState = 0f;
    private float loopTimer = 60f;


    public override void Enter()
    {
        Debug.Log("Entered Past State");
    }
    public override void Update()
    {
        timeInState += Time.deltaTime;
        if (timeInState >= loopTimer)
        {
            player.stateMachine.ChangeState(new PresentState(player));
        }
    }

    public override void Exit()
    {
    }
}
