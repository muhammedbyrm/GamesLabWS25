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
    public FirstPersonControllerInteractable fpc;
    public Image pastStateTimerImage;
    public Image pastStateTimerImageBG;
    [SerializeField] private float pastStateDuration;

    [SerializeField] private Transform flashlightPosition;
    [SerializeField] private Transform flashlightJointPosition;
    [SerializeField] private GameObject flashlight;
    [SerializeField] private AudioClip flashlightSound_on;
    [SerializeField] private AudioClip flashlightSound_off;
    private bool hasFlashlight;

    [SerializeField] GameObject BR_Door;

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PresentState());
        hasFlashlight = false;
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void InteractFlashlight()
    {
        hasFlashlight = !hasFlashlight;

        if (hasFlashlight)
        {
            flashlight.transform.GetChild(0).GetComponent<Light>().enabled = true;
            SoundManager.Instance.PlaySoundClip(flashlightSound_on, fpc.transform, 1f);
            flashlight.transform.parent = flashlightJointPosition;
            flashlight.transform.localPosition = Vector3.zero;
            flashlight.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            flashlight.transform.GetChild(0).GetComponent<Light>().enabled = false;
            SoundManager.Instance.PlaySoundClip(flashlightSound_off, fpc.transform, 2f);
            flashlight.transform.parent = flashlightPosition;
            flashlight.transform.localPosition = Vector3.zero;
            flashlight.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        flashlight.GetComponent<Flashlight>().toggleFlashlight();
    }

    public void OpenBRDoor()
    {
        BR_Door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BR_Open");
        BR_Door.transform.GetChild(1).GetComponent<Animator>().SetTrigger("BR_Open");
    }

    public void CloseBRDoor()
    {
        BR_Door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BR_Close");
        BR_Door.transform.GetChild(1).GetComponent<Animator>().SetTrigger("BR_Close");
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
