using UnityEngine;

public class MicroscopeInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject microscopeUI;
    public GameObject microscope;

    [Header("Player References")]
    public GameObject crosshair;
    public GameObject sprint;
    public MonoBehaviour playerController;

    private bool isUIOpen = false;
    private Collider triggerCollider;
    private bool wasSprintActive; 

    void Start()
    {
        if (microscopeUI != null)
            microscopeUI.SetActive(false);
        if (microscope != null)
            microscope.SetActive(false);

        triggerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isUIOpen)
        {
            if (IsPlayerInsideTrigger())
            {
                OpenMicroscope();

                GameManager.Instance.SetIncrementPastTimer(false);
                GameManager.Instance.SetPastStateTimerVisible(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isUIOpen)
        {
            CloseMicroscope();

            GameManager.Instance.escConsumedThisFrame = true;

            GameManager.Instance.SetIncrementPastTimer(true);
            GameManager.Instance.SetPastStateTimerVisible(true);
        }
    }

    private bool IsPlayerInsideTrigger()
    {
        if (triggerCollider == null)
            return false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return false;

        return triggerCollider.bounds.Contains(player.transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isUIOpen)
        {
            CloseMicroscope();
        }
    }

    private void OpenMicroscope()
    {
        isUIOpen = true;

        if (microscopeUI != null)
            microscopeUI.SetActive(true);

        if (microscope != null)
            microscope.SetActive(true);

        if (crosshair != null)
            crosshair.SetActive(false);

        if (sprint != null)
        {
            wasSprintActive = sprint.activeSelf;
            sprint.SetActive(false);
        }

        if (playerController != null)
            playerController.enabled = false;

        GameManager.Instance.isUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        var selector = FindFirstObjectByType<MicroscopeImageSelector>();
        if (selector != null)
            selector.ResetToFirstImage();
    }

    private void CloseMicroscope()
    {
        isUIOpen = false;

        if (microscopeUI != null)
            microscopeUI.SetActive(false);
        if (microscope != null)
            microscope.SetActive(false);
        if (crosshair != null)
            crosshair.SetActive(true);

        if (sprint != null)
            sprint.SetActive(wasSprintActive);

        if (playerController != null)
            playerController.enabled = true;

        GameManager.Instance.isUIOpen = false;
    }
}