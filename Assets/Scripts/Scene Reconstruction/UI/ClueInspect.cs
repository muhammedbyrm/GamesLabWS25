using UnityEngine;

public class ClueInspect : MonoBehaviour
{
    [SerializeField] private ClueData clueData;

    [Header("Outline")]
    [SerializeField] private Outline outline; // Drag your Outline component here!

    private ClueManager clueManager;

    private void Awake()
    {
        clueManager = FindObjectOfType<ClueManager>();

        // Start with outline off
        if (outline != null)
            outline.enabled = false;
    }

    public void ShowOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void Interact()
    {
        clueManager.InspectClue(clueData);
    }
}