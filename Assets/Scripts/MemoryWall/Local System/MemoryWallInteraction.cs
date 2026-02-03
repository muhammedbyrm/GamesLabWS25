using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MemoryWallInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject informationText;
    public GameObject memoryWallUI;
    public GameObject noteSystemUI;

    [Header("Unlock Colliders")]
    [SerializeField] private InteractionUnlocker interactionUnlocker;
    private bool interactionsUnlocked = false;

    [Header("Player Reference")]
    public MonoBehaviour playerController;
    public GameObject crosshair;

    private bool isPlayerInZone = false;
    private bool isUIOpen = false;
    private bool hasOpenedOnce = false;

    void Start()
    {
        if (informationText != null)
            informationText.SetActive(false);

    }

    void Update()
    {
        if (isPlayerInZone && !isUIOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenUI();
        }

        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
            GameManager.Instance.escConsumedThisFrame = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

            if (informationText != null)
                informationText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            if (informationText != null)
                informationText.SetActive(false);

            if (isUIOpen)
                CloseUI();
        }
    }

    private void OpenUI()
    {
        GameManager.Instance.SetIncrementPastTimer(false);
        GameManager.Instance.SetPastStateTimerVisible(false);

        isUIOpen = true;

        if (informationText != null)
            informationText.SetActive(false);

        if (memoryWallUI != null)
            memoryWallUI.SetActive(true);

        if (noteSystemUI != null)
            noteSystemUI.SetActive(true);

        if (playerController != null)
            playerController.enabled = false;

        if (crosshair != null)
            crosshair.SetActive(false);

        GameManager.Instance.isUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!GameManager.Instance.memoryWallInteraction)
            GameManager.Instance.memoryWallInteraction = true;

        TaskUIManager.Instance.ChangeVisbility(false);
    }

    public void CloseUI()
    {
        GameManager.Instance.SetIncrementPastTimer(true);
        if(GameManager.Instance.GetIsInThePast())
            GameManager.Instance.SetPastStateTimerVisible(true);

        isUIOpen = false;

        if (memoryWallUI != null)
            memoryWallUI.SetActive(false);

        if (noteSystemUI != null)
            noteSystemUI.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        if (crosshair != null)
            crosshair.SetActive(true);

        GameManager.Instance.isUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (isPlayerInZone && informationText != null)
            informationText.SetActive(true);


        if (!hasOpenedOnce && GameManager.Instance.memoryWallMessageRead)
        {
            hasOpenedOnce = true;
            GameManager.Instance.UIAfterMemoryWall();
            TaskUIManager.Instance.ChangeTask(2);
        }

        TaskUIManager.Instance.ChangeVisbility(true);
    }
}