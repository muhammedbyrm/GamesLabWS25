using System.Threading;
using UnityEngine;

public class State
{
    protected Player player;

    public State(Player player)
    {
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

}
