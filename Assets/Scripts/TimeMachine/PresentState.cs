using System.Threading;
using UnityEngine;

public class PresentState : State
{

    public PresentState(Player player) : base(player) { }


    public override void Enter()
    {
        Debug.Log("Entered Present State");

    }
    public override void Update()
    {


    }

    public override void Exit()
    {
        player.TimeJump();
    }
}
