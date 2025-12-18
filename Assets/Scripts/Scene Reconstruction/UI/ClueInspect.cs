using UnityEngine;

public class ClueInspect : MonoBehaviour
{
    [SerializeField] private ClueData clueData; // Drag asset

    [Header("Outline")]
    [SerializeField] private Outline outline; // Drag Outline component (self-reference)

    private ClueManager clueManager;

    private void Start()
    {
        clueManager = FindObjectOfType<ClueManager>();
        if (outline != null) outline.SetOutline(false);
    }

    // Called by raycast hit
    public void Highlight(bool highlight)
    {
        if (outline != null) outline.SetOutline(highlight);
    }

    // Called on E press
    public void Interact()
    {
        clueManager.InspectClue(clueData);
    }
}   