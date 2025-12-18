using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask clueLayer = 1 << 6; // Create Layer 6 "Clues"

    private ClueInspect currentClue;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>() ?? Camera.main;
    }

    private void Update()
    {
        Raycast();

        if (Input.GetKeyDown(KeyCode.E) && currentClue != null)
        {
            currentClue.Interact();
        }
    }

    private void Raycast()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Middle screen

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, clueLayer))
        {
            ClueInspect hitClue = hit.collider.GetComponent<ClueInspect>();
            if (hitClue != null && hitClue != currentClue)
            {
                // New clue
                if (currentClue != null) currentClue.Highlight(false);
                currentClue = hitClue;
                currentClue.Highlight(true); // Outline glows!
            }
        }
        else
        {
            // No hit
            if (currentClue != null)
            {
                currentClue.Highlight(false);
                currentClue = null;
            }
        }
    }
}