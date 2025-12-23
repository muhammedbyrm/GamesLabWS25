using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private LayerMask clueLayer;

    private ClueInspect currentClue;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>() ?? Camera.main;
    }

    private void Update()
    {
        CheckLookAtClue();

        if (Input.GetKeyDown(KeyCode.E) && currentClue != null)
        {
            currentClue.Interact();
        }
    }

    private void CheckLookAtClue()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, clueLayer))
        {
            ClueInspect clue = hit.collider.GetComponent<ClueInspect>();
            if (clue != null && clue != currentClue)
            {
                // New clue in view
                if (currentClue != null) currentClue.HideOutline();
                currentClue = clue;
                currentClue.ShowOutline();
            }
        }
        else
        {
            // Looking at nothing
            if (currentClue != null)
            {
                currentClue.HideOutline();
                currentClue = null;
            }
        }
    }
}