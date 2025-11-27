using UnityEngine;

public class Player : MonoBehaviour
{
    #region Player Stat

    public int sanity = 100;
    // How much each time jump reduces sanity
    public int sanityReduction = 10;



    public StateMachine stateMachine { get; private set; }

    #endregion


    private int loopCount = 0;

    SanityManager sanityManager;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PresentState(this));
    }

    // Update is called once per frame
    void Update()
    {

        stateMachine.Update();
    }

    public void TimeJump()
    {
        loopCount++;
        
    }

    void test()
    {
        
    }
}
