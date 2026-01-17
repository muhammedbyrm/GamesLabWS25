using NavKeypad;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


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
    public GameObject livingCompanion;
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
    public ReconstructionController reconstructionController;
    public Transform playerResetLocation;
    public AudioClip blackout;
    public AudioClip cameraShake;
    public Image blackscreen;
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
    public bool memoryWallInteraction = false;

    private bool murderWeaponUIHasBeenRead = false;
    private bool incremenentPastTimer = true;

    private bool hasDestroyedKnife = false;

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PresentState());
        hasFlashlight = false;
        hasReconstructedScene = false;
        hasDroppedMurderWeapon = false;
        hasMurderWeapon = false;
        blackscreen.color = new Color(0f, 0f, 0f, 0f);
    }

    void Update()
    {
        stateMachine.Update();
    }

    #region flashlight
    public void InteractFlashlight()
    {
        hasFlashlight = !hasFlashlight;

        if (hasFlashlight)
        {
            flashlightSettings.flashlightPosition.GetComponent<Outline>().enabled = false;
            flashlightSettings.flashlight.transform.GetChild(0).GetComponent<Light>().enabled = true;
            SoundManager.Instance.PlaySoundClip(flashlightSettings.flashlightSound_on, fpc.transform, 1f);
            flashlightSettings.flashlight.transform.SetParent(flashlightSettings.flashlightJointPosition, false);
            flashlightSettings.flashlight.transform.localScale = Vector3.one;
            flashlightSettings.flashlight.transform.localPosition = Vector3.zero;
            flashlightSettings.flashlight.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            flashlightSettings.flashlightPosition.GetComponent<Outline>().enabled = true;
            flashlightSettings.flashlight.transform.GetChild(0).GetComponent<Light>().enabled = false;
            SoundManager.Instance.PlaySoundClip(flashlightSettings.flashlightSound_off, fpc.transform, 2f);
            flashlightSettings.flashlight.transform.SetParent(flashlightSettings.flashlightPosition, false);
            flashlightSettings.flashlight.transform.localScale = Vector3.one;
            flashlightSettings.flashlight.transform.localPosition = Vector3.zero;
            flashlightSettings.flashlight.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        flashlightSettings.flashlight.GetComponent<Flashlight>().toggleFlashlight();
    }

    public bool GetHasFlashlight()
    {
        return hasFlashlight;
    }
    #endregion flashlight

    #region murder Weapon
    public void PickUpMurderWeapon()
    {
        if(murderWeaponUIHasBeenRead == false)
        {
            murderWeaponUIHasBeenRead = true;
            UIpickUpMurderWeapon();
        }

        murderWeaponSettings.knife.transform.SetParent(murderWeaponSettings.knifeJointPosition, false);
        murderWeaponSettings.knife.transform.localScale = new Vector3(2.17f, 2.17f, 2.17f);
        //murderWeaponSettings.knife.transform.parent = murderWeaponSettings.knifeJointPosition;
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
                murderWeaponSettings.knife.transform.SetParent(murderWeaponSettings.knifeDropLocation, false);
                murderWeaponSettings.knife.transform.localScale = new Vector3(2.17f, 2.17f, 2.17f);
                //murderWeaponSettings.knife.transform.parent = murderWeaponSettings.knifeDropLocation;
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
        murderWeaponSettings.knife.transform.SetParent(murderWeaponSettings.knifePosition, false);
        murderWeaponSettings.knife.transform.localScale = new Vector3(2.17f, 2.17f, 2.17f);
        //murderWeaponSettings.knife.transform.parent = murderWeaponSettings.knifePosition;
        murderWeaponSettings.knife.transform.localPosition = Vector3.zero;
        murderWeaponSettings.knife.transform.localRotation = Quaternion.Euler(Vector3.zero);
        hasMurderWeapon = false;
    }
    #endregion murder Weapon

    #region UI
    public void UImemoryWall()
    {
        reconstructionController.BeforeMemoryWallUI();
    }

    public void UIstartGame()
    {
        reconstructionController.StartGameUI();
    }

    public void UIopenBRDoor()
    {
        reconstructionController.OpeningDoorUI();
    }

    public void UIpickUpMurderWeapon()
    {
        reconstructionController.MurderWeaponUI();
    }

    public void UIAfterFirstTimeJump()
    {
        reconstructionController.AfterFirstTimeJump();
    }
    public void UIAfterSecondTimeJump()
    {
        reconstructionController.AfterSecondTimeJump();
    }
    public void UIAfterThirdTimeJump()
    {
        reconstructionController.AfterThirdTimeJump();
    }
    #endregion UI

    public bool getIncrementPastTimer()
    {
        return incremenentPastTimer;
    }

    public void SetIncrementPastTimer(bool incrementPastTimer)
    {
        this.incremenentPastTimer = incrementPastTimer;
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
    public void OpenBRDoor(bool playsound)
    {
        //Debug.Log("Open BR Door");
        if (
        br_Door.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened") ||
        br_Door.transform.GetChild(1).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened"))
        {

        }
        else
        {

            br_Door.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BR_Open");
            br_Door.transform.GetChild(1).GetComponent<Animator>().SetTrigger("BR_Open");
        }

        if (playsound == true) { 
            SoundManager.Instance.PlaySoundClip(br_Door_Open, br_Door.transform, 1f);
        }

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
        OpenBRDoor(true);
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

    #region blackout sequence
    public void MovePlayer()
    {
        fpc.transform.position = playerResetLocation.position;
        fpc.transform.rotation = playerResetLocation.rotation;
    }

    private void Blackout()
    {
        fpc.playerCanMove = false;
        foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            light.enabled = false;
        }
        RenderSettings.ambientIntensity = 0f;
        SoundManager.Instance.PlaySoundClip(blackout, fpc.transform, 1f);
    }

    private void CameraShake()
    {
        fpc.GetComponentInChildren<CameraShake>().Shake(5f, 1f);
        SoundManager.Instance.PlaySoundClip(cameraShake, fpc.transform, 1f);
    }

    private void CameraFadeOut()
    {
        blackscreen.color = new Color(0f, 0f, 0f, 1f);
    }
    public void LightOn()
    {
        foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            light.enabled = true;
        }
        RenderSettings.ambientIntensity = 1f;
    }

    private IEnumerator CameraFadeIn()
    {
        float timer = 0f;
        float alpha = 1f;
        while(timer < 2f)
        {
            timer += Time.deltaTime;
            alpha = Mathf.Lerp(alpha, 0f, timer / 2f);
            blackscreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fpc.playerCanMove = true;
        blackscreen.color = new Color(0f, 0f, 0f, 0f);
    }

    private IEnumerator PastToPresent()
    {
        SoundManager.Instance.PlayBGMChoose(4);

        GameManager.Instance.Blackout();
        yield return new WaitForSeconds(3f);

        GameManager.Instance.CameraShake();
        yield return new WaitForSeconds(5f);

        GameManager.Instance.CameraFadeOut();
        yield return new WaitForSeconds(2f);

        GameManager.Instance.stateMachine.ChangeState(new PresentState());
    }

    public void StartFadeIn()
    {
        StartCoroutine(CameraFadeIn());
    }

    public void StartPastToPresentSequence()
    {
        StartCoroutine(PastToPresent());
    }
    #endregion blackout sequence

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

    #region state switch objects
    public void activatePresentObjects()
    {
        if (GetHasDestroyedMurderWeapon()){
            StartFinalSequence();
            DeactivateTMButton();
        }
        else{ 
        reconstructionSettings.bodyBlood.SetActive(true);
        reconstructionSettings.bloodSplatter.SetActive(true);
        reconstructionSettings.bloodyKnife.SetActive(true);
        reconstructionSettings.struggleSign.SetActive(true);
        reconstructionSettings.bloodyTrail.SetActive(true);
        reconstructionSettings.companion.SetActive(true);
        }
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
        reconstructionSettings.livingCompanion.SetActive(true);
    }

    public void deactivatePastObjects()
    {
        reconstructionSettings.pastChair.SetActive(false);
        murderWeaponSettings.knifePosition.gameObject.SetActive(false);
        reconstructionSettings.livingCompanion.SetActive(false);
    }
    #endregion state switch objects

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

    public void HasDestroyedMurderWeapon()
    {
         hasDestroyedKnife = true;
         StartCoroutine(jumpToPresent());
    }

    private IEnumerator jumpToPresent()
    {
        SoundManager.Instance.PlayBGMChoose(4);
        yield return new WaitForSeconds(7f);

        GameManager.Instance.Blackout();
        yield return new WaitForSeconds(3f);

        GameManager.Instance.CameraShake();
        yield return new WaitForSeconds(5f);

        GameManager.Instance.CameraFadeOut();
        yield return new WaitForSeconds(2f);

        GameManager.Instance.stateMachine.ChangeState(new PresentState());
    }

    public bool GetHasDestroyedMurderWeapon()
    {
        return hasDestroyedKnife;
    }
    private void FinishFinalSequence()
    {
        EndGame();
    }
}
