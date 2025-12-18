using UnityEngine;

public class MicroscopeInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject infoText;        
    public GameObject microscopeUI;    
    public GameObject microscope;  

    [Header("Player References")]
    public GameObject crosshair;
    public GameObject canvasHUD;        

    private bool isPlayerInZone = false;
    private bool isUIOpen = false;

    void Start()
    {
        if (infoText != null)
            infoText.SetActive(false);

        if (microscopeUI != null)
            microscopeUI.SetActive(false);

        if (microscope != null)
            microscope.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInZone && !isUIOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenMicroscope();
        }

        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMicroscope();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

            if (infoText != null)
                infoText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            if (infoText != null)
                infoText.SetActive(false);

            if (isUIOpen)
                CloseMicroscope();
        }
    }

    private void OpenMicroscope()
    {
        isUIOpen = true;

        if (infoText != null)
            infoText.SetActive(false);

        if (microscopeUI != null)
            microscopeUI.SetActive(true);

        if (microscope != null)
            microscope.SetActive(true);

        var selector = FindFirstObjectByType<MicroscopeImageSelector>();
        if (selector != null)
            selector.ResetToFirstImage();

        if (crosshair != null)
            crosshair.SetActive(false);

        if (canvasHUD != null)
            canvasHUD.SetActive(false);
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

        if (canvasHUD != null)
            canvasHUD.SetActive(true);

        if (isPlayerInZone && infoText != null)
            infoText.SetActive(true);
    }
}
