using NavKeypad;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject companion;

    [Space(10)]
    public GameObject pastChair;
}

[System.Serializable]
public class MurderWeaponSettings
{
    public Transform knifePosition;
    public Transform knifeJointPosition;
    public Transform knifeDropLocation;
    public GameObject knife;
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

    [Header("Murder Weapon Settings")]
    [SerializeField] private MurderWeaponSettings murderWeaponSettings;
    private bool hasDroppedMurderWeapon;
    private bool hasMurderWeapon;

    [Header("Fade Out UI")]
    [SerializeField] private GameEndController gameEndController;
    [SerializeField] private GameObject reticle;
    [SerializeField] GameObject canvasHUD;

    [Header("Companion")]
    [SerializeField] private GameObject companionRoot;

    [Header("Elements")]
    [SerializeField] private GameObject br_Door;
    [SerializeField] private AudioClip br_Door_Open;
    [SerializeField] private Keypad keypad;
    [SerializeField] private GameObject timeMachineButton;


    [Header("Story Flags")]
    public bool memoryWallMessageRead = false;
    public bool ScienceMessageRead = false;


    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PresentState());
        hasFlashlight = false;
        hasReconstructedScene = false;
        hasDroppedMurderWeapon = false;
        hasMurderWeapon = false ;
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
    public void PickUpMurderWeapon()
    {
        murderWeaponSettings.knife.transform.parent = murderWeaponSettings.knifeJointPosition;
        murderWeaponSettings.knife.transform.localPosition = Vector3.zero;
        murderWeaponSettings.knife.transform.localRotation = Quaternion.Euler(Vector3.zero);
        hasMurderWeapon = true;
    }

    public void DropMurderWeapon()
    {
        if (hasMurderWeapon)
        {
            if (!hasDroppedMurderWeapon)
            {
                hasDroppedMurderWeapon = true;
                murderWeaponSettings.knife.transform.parent = murderWeaponSettings.knifeDropLocation;
                murderWeaponSettings.knife.transform.localPosition = Vector3.zero;
                murderWeaponSettings.knife.transform.localRotation = Quaternion.Euler(Vector3.zero);
                ActivateTMButton();
                hasMurderWeapon = false;
            }
        }
    }

    public bool GetHasDroppedMurderWeapon()
    {
        return hasDroppedMurderWeapon;
    }

    public void ReturnMurderWeapon()
    {
        hasDroppedMurderWeapon = false;
        murderWeaponSettings.knifePosition.GetComponent<Collider>().gameObject.SetActive(true);
        murderWeaponSettings.knife.transform.parent = murderWeaponSettings.knifePosition;
        murderWeaponSettings.knife.transform.localPosition = Vector3.zero;
        murderWeaponSettings.knife.transform.localRotation = Quaternion.Euler(Vector3.zero);
        hasMurderWeapon = false;
    }

    
    public void EndGame()
    {
        // end game animation will come there ...


        //


        if (reticle != null)
            reticle.SetActive(false);

        if (canvasHUD != null)
            canvasHUD.SetActive(false);

        gameEndController.StartFadeOut();
    }

    public void ActivateTMButton()
    {
        timeMachineButton.GetComponent<Collider>().enabled = true;
    }

    public void DeactivateTMButton()
    {
        timeMachineButton.GetComponent<Collider>().enabled = false;
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
        reconstructionSettings.companion.SetActive(true);
    }

    public void deactivatePresentObjects()
    {
        reconstructionSettings.bodyBlood.SetActive(false);
        reconstructionSettings.bloodSplatter.SetActive(false);
        reconstructionSettings.bloodyKnife.SetActive(false);
        reconstructionSettings.struggleSign.SetActive(false);
        reconstructionSettings.bloodyTrail.SetActive(false);
        reconstructionSettings.companion.SetActive(false);
    }

    public void activatePastObjects()
    {
        reconstructionSettings.pastChair.SetActive(true);
        murderWeaponSettings.knifePosition.gameObject.SetActive(true);
    }

    public void deactivatePastObjects()
    {
        reconstructionSettings.pastChair.SetActive(false);
        murderWeaponSettings.knifePosition.gameObject.SetActive(false);
    }

    public void OnCompanionArrived()
    {
        Debug.Log("Companion arrived, starting dialogue.");

        // 3 sec for speaking
        Invoke(nameof(FinishFinalSequence), 3f);
    }
    public void StartFinalSequence()
    {
        Debug.Log("Final sequence started");

        if (companionRoot != null)
        {
            companionRoot.SetActive(true);
        }
        else
        {
            Debug.LogError("compainRoot is NOT assigned in GameManager!");
        }
    }

    private void FinishFinalSequence()
    {
        EndGame();
    }
}
