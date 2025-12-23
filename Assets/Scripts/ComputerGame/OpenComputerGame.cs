using UnityEngine;

public class OpenComputerGame : MonoBehaviour
{
    [SerializeField] private BubbleSortPuzzle puzzleScript; 
    [SerializeField] private Outline outline; 

    private void Start()
    {
        // DO NOT disable the outline script here. 
        // It needs to be enabled to run its distance/raycast logic.
        if (outline == null) outline = GetComponent<Outline>();
    }

    private void Update()
    {
        // Logic: If the outline script is showing the 'Press E' text, 
        // it means the distance and raycast checks have passed.
        if (outline != null && outline.interactionText != null)
        {
            if (outline.interactionText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    public void Interact()
    {
        if (puzzleScript != null)
        {
            puzzleScript.OpenPuzzle();
            // Optional: Hide the text immediately so it doesn't stay on screen
            if(outline.interactionText != null) 
                outline.interactionText.gameObject.SetActive(false);
        }
    }
}