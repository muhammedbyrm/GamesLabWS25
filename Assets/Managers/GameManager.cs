using NavKeypad;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class FlashlightSettings
{
    public Transform flashlightPosition;
    public Transform flashlightJointPosition;
    public GameObject flashlight;
    public AudioClip flashlightSound_on;
    public AudioClip flashlightSound_off;
}

[System.Serializable]
public class SceneReconstructionSettings
{
    public GameObject bodyBlood;
    public GameObject bloodSplatter;
    public GameObject struggleSign;
    public GameObject bloodyTrail;
    public GameObject bloodyKnife;

    [Space(10)]
    public GameObject pastChair;
}

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

    [Header("Flashlight Settings")]
    [SerializeField] private FlashlightSettings flashlightSettings;
    private bool hasFlashlight;

    [Header("Scene Reconstruction Settings")]
    [SerializeField] private SceneReconstructionSettings reconstructionSettings;
    private bool hasReconstructedScene;

    [SerializeField] private GameObject br_Door;
    [SerializeField] private AudioClip br_Door_Open;
    [SerializeField] private Keypad keypad;


    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PresentState());
        hasFlashlight = false;
        hasReconstructedScene = false;
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
            flashlightSettings.flashlight.transform.GetChild(0).GetComponent<Light>().enabled = true;
            SoundManager.Instance.PlaySoundClip(flashlightSettings.flashlightSound_on, fpc.transform, 1f);
            flashlightSettings.flashlight.transform.parent = flashlightSettings.flashlightJointPosition;
            flashlightSettings.flashlight.transform.localPosition = Vector3.zero;
            flashlightSettings.flashlight.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            flashlightSettings.flashlight.transform.GetChild(0).GetComponent<Light>().enabled = false;
            SoundManager.Instance.PlaySoundClip(flashlightSettings.flashlightSound_off, fpc.transform, 2f);
            flashlightSettings.flashlight.transform.parent = flashlightSettings.flashlightPosition;
            flashlightSettings.flashlight.transform.localPosition = Vector3.zero;
            flashlightSettings.flashlight.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        flashlightSettings.flashlight.GetComponent<Flashlight>().toggleFlashlight();
    }

    public void OpenBRDoor()
    {
        //Debug.Log("Open BR Door");
        if(
        br_Door.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened") ||
        br_Door.transform.GetChild(1).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened")){

        }else
        {

            br_Door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BR_Open");
            br_Door.transform.GetChild(1).GetComponent<Animator>().SetTrigger("BR_Open");
        }

        SoundManager.Instance.PlaySoundClip(br_Door_Open, br_Door.transform, 1f);

        //BR_Door.transform.GetChild(0).GetComponent<Animator>().ResetTrigger("BR_Open");
        //BR_Door.transform.GetChild(1).GetComponent<Animator>().ResetTrigger("BR_Open");
    }

    public void CloseBRDoor()
    {
        //Debug.Log("Close BR Door");
        br_Door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BR_Close");
        br_Door.transform.GetChild(1).GetComponent<Animator>().SetTrigger("BR_Close");


        //BR_Door.transform.GetChild(0).GetComponent<Animator>().ResetTrigger("BR_Close");
        //BR_Door.transform.GetChild(1).GetComponent<Animator>().ResetTrigger("BR_Close");
    }


    public void FinishSceneReconstruction()
    {
        hasReconstructedScene = true;
        OpenBRDoor();
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

    public void ActivateKeypad()
    {
        //keypad.enabled = true;
        keypad.ResetKeypad();
        foreach (var collider in keypad.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
    }

    public void DeactivateKeypad()
    {
        //keypad.enabled = false;
        keypad.ResetKeypad();
        foreach (var collider in keypad.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    public void activatePresentObjects()
    {
        reconstructionSettings.bodyBlood.SetActive(true);
        reconstructionSettings.bloodSplatter.SetActive(true);
        reconstructionSettings.bloodyKnife.SetActive(true);
        reconstructionSettings.struggleSign.SetActive(true);
        reconstructionSettings.bloodyTrail.SetActive(true);
    }

    public void deactivatePresentObjects()
    {
        reconstructionSettings.bodyBlood.SetActive(false);
        reconstructionSettings.bloodSplatter.SetActive(false);
        reconstructionSettings.bloodyKnife.SetActive(false);
        reconstructionSettings.struggleSign.SetActive(false);
        reconstructionSettings.bloodyTrail.SetActive(false);
    }

    public void activatePastObjects()
    {
        reconstructionSettings.pastChair.SetActive(true);
    }

    public void deactivatePastObjects()
    {
        reconstructionSettings.pastChair.SetActive(false);
    }
}
