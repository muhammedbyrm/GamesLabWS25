using System.Threading;
using UnityEngine;

public class State
{

    public State()
    {
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

}
