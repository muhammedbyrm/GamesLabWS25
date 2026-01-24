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
            puzzleController.ActivatePuzzle();
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