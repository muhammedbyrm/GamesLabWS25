using UnityEngine;

public class GoldenFrame_Terminal_FREE : MonoBehaviour
{
    [Header("Terminal Visuals")]
    public GameObject textMesh;
    public MeshRenderer screenRenderer;
    public Material emissiveMaterial;
    public Material normalMaterial;

    [Header("Puzzle Settings")]
    public LaserPuzzleController puzzleController;
    
    [Header("Floating UI")]
    public GameObject interactionUI; 
    public float interactionDistance = 3f;

    [Header("Puzzle Description UI")]
    [SerializeField] private InspectionUIController inspectionUI; // <-- Drag the obj from BREAKROOM/UI/InspectionControllerUI from the Hierarchy

    private bool isOn = false;
    private Transform playerTransform;

    private void Start()
    {
        TurnOffComputer();
        // Don't just cache the transform once, because the Main Camera 
        // gets disabled/enabled during the puzzle.
    }

    private void Update()
    {
        // Always try to find the Main Camera if our reference is null or disabled
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            if (Camera.main != null) playerTransform = Camera.main.transform;
        }

        // Keep the floating text facing the player
        if (interactionUI != null && interactionUI.activeSelf && playerTransform != null)
        {
            interactionUI.transform.LookAt(interactionUI.transform.position + playerTransform.forward);
            
            // Safety: Hide UI if player walks away while it's open
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist > interactionDistance)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    private void OnMouseOver() // Using Over instead of Enter for more reliability
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= interactionDistance && interactionUI != null)
        {
            // Only show UI if we aren't already in the puzzle
            interactionUI.SetActive(true);
        }
        else if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > interactionDistance) return;

        // Toggle computer visuals
        if (isOn) TurnOffComputer();
        else TurnOnComputer();

        // Start Puzzle
        if (puzzleController != null)
        {
            if (inspectionUI != null)
            {
                inspectionUI.Show(
                    "Puzzle Description", 
                    "Hit all receivers with the laser beam to reveal the digit. \n Press 1 to select a mirror to deflect the beam. Press 2 to select a splitter to duplicate the beam. \n Press Left Click to place selected object on the table and press the placed object to rotate the beam. \n Right click the object to delete it."
                );
                
                if (Input.GetKeyDown(KeyCode.K))
                {
                    Debug.Log("InspectionUI: Escape pressed — closing UI");
                    inspectionUI.setIsVisible(false);
                }
            }
            
            
            puzzleController.ActivatePuzzle();
            // Force UI off immediately when entering puzzle
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }

    private void TurnOnComputer()
    {
        if (screenRenderer != null && emissiveMaterial != null)
        {
            Material[] mats = screenRenderer.materials;
            mats[1] = emissiveMaterial; 
            screenRenderer.materials = mats;
        }
        if (textMesh != null) textMesh.SetActive(true);
        isOn = true;
    }

    private void TurnOffComputer()
    {
        if (screenRenderer != null && normalMaterial != null)
        {
            Material[] mats = screenRenderer.materials;
            mats[1] = normalMaterial;
            screenRenderer.materials = mats;
        }
        if (textMesh != null) textMesh.SetActive(false);
        isOn = false;
    }
}