using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region GameManager
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NUll");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        Debug.Log("Hello I am your personal GameManager!");
    }
    #endregion
    public StateMachine stateMachine { get; private set; }
    private int loopCount = 0;

    public GameObject sphere;
    public GameObject cube;
    public SanityManager sanityManager;
    public Image pastStateTimerImage;
    public Image pastStateTimerImageBG;
    [SerializeReference] private float pastStateDuration;

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PresentState());
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void TimeJump()
    {
        loopCount++;
    }

    public int GetLoopCount()
    {
        return loopCount;
    }

    public float GetPastStateDuration()
    {
        return pastStateDuration;
    }
}
